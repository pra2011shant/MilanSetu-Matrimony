import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable, BehaviorSubject, Subject } from 'rxjs';

export interface ChatMessage {
  id: number;
  senderId: number;
  receiverId: number;
  content: string;
  isRead: boolean;
  sentAt: string;
  isMine: boolean;
}

export interface ConversationSummary {
  userId: number;
  profileId: string;
  name: string;
  imageUrl: string;
  location: string;
  profession: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadCount: number;
  isOnline: boolean;
  isVerified: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubUrl = 'http://localhost:5000/hubs/chat';
  private apiUrl = 'http://localhost:5000/api/chat';

  private hubConnection: signalR.HubConnection | null = null;
  public messageReceived$ = new Subject<ChatMessage>();
  public userTyping$ = new Subject<{ senderId: number; isTyping: boolean }>();
  public isConnected$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {
    this.initSignalRConnection();
  }

  private initSignalRConnection(): void {
    try {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl, {
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect()
        .build();

      this.hubConnection.on('ReceiveMessage', (data: any) => {
        const msg: ChatMessage = {
          id: data.id,
          senderId: data.senderId,
          receiverId: data.receiverId,
          content: data.content,
          isRead: data.isRead,
          sentAt: data.sentAt,
          isMine: false
        };
        this.messageReceived$.next(msg);
      });

      this.hubConnection.on('MessageSentConfirmation', (data: any) => {
        const msg: ChatMessage = {
          id: data.id,
          senderId: data.senderId,
          receiverId: data.receiverId,
          content: data.content,
          isRead: data.isRead,
          sentAt: data.sentAt,
          isMine: true
        };
        this.messageReceived$.next(msg);
      });

      this.hubConnection.on('UserTyping', (data: { senderId: number; isTyping: boolean }) => {
        this.userTyping$.next(data);
      });

      this.hubConnection.start()
        .then(() => {
          this.isConnected$.next(true);
        })
        .catch(() => {
          this.isConnected$.next(false);
        });

      this.hubConnection.onclose(() => {
        this.isConnected$.next(false);
      });
    } catch {
      this.isConnected$.next(false);
    }
  }

  getConversations(): Observable<ConversationSummary[]> {
    return this.http.get<ConversationSummary[]>(`${this.apiUrl}/conversations`);
  }

  getMessages(otherUserId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/messages/${otherUserId}`);
  }

  async sendRealtimeMessage(receiverId: number, content: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('SendMessage', receiverId, content);
    } else {
      // Fallback REST call
      this.http.post<ChatMessage>(`${this.apiUrl}/send`, { receiverId, content }).subscribe({
        next: (msg) => {
          this.messageReceived$.next(msg);
        }
      });
    }
  }

  async sendTypingStatus(receiverId: number, isTyping: boolean): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('SendTypingIndicator', receiverId, isTyping);
      } catch {
        // Silently catch if disconnected
      }
    }
  }
}
