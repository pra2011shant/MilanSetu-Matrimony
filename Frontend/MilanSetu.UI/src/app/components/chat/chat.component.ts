import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ChatService, ChatMessage, ConversationSummary } from '../../services/chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesScrollContainer') private scrollContainer!: ElementRef;

  conversations: ConversationSummary[] = [];
  activeConversation: ConversationSummary | null = null;
  messages: ChatMessage[] = [];
  newMessageText = '';
  isRecipientTyping = false;
  isLoadingConversations = true;
  isLoadingMessages = false;
  searchFilter = '';

  private subs = new Subscription();
  private typingTimeout: any;

  // Icebreaker Quick Prompts
  quickIcebreakers = [
    "Namaste! 🙏 Glad to connect with you on MilanSetu.",
    "I really liked reading your profile bio!",
    "Are you free for a voice call this weekend?",
    "What kind of hobbies do you enjoy the most?"
  ];

  constructor(public chatService: ChatService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loadConversations();
    this.listenToRealtimeEvents();
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  loadConversations(): void {
    this.isLoadingConversations = true;
    this.chatService.getConversations().subscribe({
      next: (data) => {
        this.isLoadingConversations = false;
        this.conversations = data;
        
        // Auto-select first conversation or query param user
        this.route.queryParams.subscribe(params => {
          const targetUid = +params['userId'];
          if (targetUid) {
            const match = this.conversations.find(c => c.userId === targetUid);
            if (match) {
              this.selectConversation(match);
              return;
            }
          }
          if (this.conversations.length > 0 && !this.activeConversation) {
            this.selectConversation(this.conversations[0]);
          }
        });
      },
      error: () => {
        this.isLoadingConversations = false;
      }
    });
  }

  selectConversation(conv: ConversationSummary): void {
    this.activeConversation = conv;
    conv.unreadCount = 0;
    this.isLoadingMessages = true;
    this.messages = [];

    this.chatService.getMessages(conv.userId).subscribe({
      next: (msgs) => {
        this.isLoadingMessages = false;
        this.messages = msgs;
      },
      error: () => {
        this.isLoadingMessages = false;
      }
    });
  }

  listenToRealtimeEvents(): void {
    this.subs.add(
      this.chatService.messageReceived$.subscribe((msg) => {
        if (this.activeConversation && (msg.senderId === this.activeConversation.userId || (msg.isMine && msg.receiverId === this.activeConversation.userId))) {
          this.messages.push(msg);
          this.activeConversation.lastMessage = msg.content;
          this.activeConversation.lastMessageTime = msg.sentAt;
        } else {
          const conv = this.conversations.find(c => c.userId === msg.senderId);
          if (conv) {
            conv.lastMessage = msg.content;
            conv.lastMessageTime = msg.sentAt;
            conv.unreadCount++;
          }
        }
      })
    );

    this.subs.add(
      this.chatService.userTyping$.subscribe((data) => {
        if (this.activeConversation && data.senderId === this.activeConversation.userId) {
          this.isRecipientTyping = data.isTyping;
        }
      })
    );
  }

  onTyping(): void {
    if (!this.activeConversation) return;
    this.chatService.sendTypingStatus(this.activeConversation.userId, true);

    clearTimeout(this.typingTimeout);
    this.typingTimeout = setTimeout(() => {
      if (this.activeConversation) {
        this.chatService.sendTypingStatus(this.activeConversation.userId, false);
      }
    }, 2000);
  }

  async sendMessage(textToSend?: string): Promise<void> {
    const text = (textToSend || this.newMessageText).trim();
    if (!text || !this.activeConversation) return;

    this.newMessageText = '';
    await this.chatService.sendRealtimeMessage(this.activeConversation.userId, text);
    if (this.activeConversation) {
      this.activeConversation.lastMessage = text;
      this.activeConversation.lastMessageTime = new Date().toISOString();
    }
  }

  get filteredConversations(): ConversationSummary[] {
    if (!this.searchFilter.trim()) return this.conversations;
    return this.conversations.filter(c =>
      c.name.toLowerCase().includes(this.searchFilter.toLowerCase()) ||
      c.profession.toLowerCase().includes(this.searchFilter.toLowerCase())
    );
  }

  private scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch {}
  }
}
