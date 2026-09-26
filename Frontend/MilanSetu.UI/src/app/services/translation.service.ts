import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

export interface LanguageOption {
  code: string;
  name: string;
  nativeName: string;
  flag: string;
}

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private apiUrl = 'http://localhost:5000/api/auth/language';

  public supportedLanguages: LanguageOption[] = [
    { code: 'en', name: 'English', nativeName: 'English', flag: '🇬🇧' },
    { code: 'hi', name: 'Hindi', nativeName: 'हिन्दी', flag: '🇮🇳' },
    { code: 'bn', name: 'Bengali', nativeName: 'বাংলা', flag: '🇮🇳' },
    { code: 'mr', name: 'Marathi', nativeName: 'मराठी', flag: '🇮🇳' },
    { code: 'ta', name: 'Tamil', nativeName: 'தமிழ்', flag: '🇮🇳' },
    { code: 'te', name: 'Telugu', nativeName: 'తెలుగు', flag: '🇮🇳' },
    { code: 'gu', name: 'Gujarati', nativeName: 'ગુજરાતી', flag: '🇮🇳' },
    { code: 'kn', name: 'Kannada', nativeName: 'ಕನ್ನಡ', flag: '🇮🇳' }
  ];

  private currentLangSubject = new BehaviorSubject<string>(this.getStoredLanguage());
  public currentLang$: Observable<string> = this.currentLangSubject.asObservable();

  private translations: Record<string, Record<string, string>> = {
    en: {
      'nav.home': 'Home',
      'nav.matches': 'Matches',
      'nav.interests': 'Interests',
      'nav.chat': 'Chat',
      'nav.shortlist': 'Shortlist',
      'nav.search': 'Search',
      'nav.preferences': 'Preferences',
      'nav.profile': 'Profile',
      'nav.login': 'Login',
      'nav.registerFree': 'Register Free',
      'nav.logout': 'Logout',
      'nav.language': 'Language',

      'hero.badge': '✨ India’s Most Trusted Sacred Matrimony',
      'hero.titlePart1': 'Find Your',
      'hero.titleHighlight': 'Perfect Life Partner',
      'hero.subtitle': 'Over 500,000+ verified Hindu, Sikh, Jain, Muslim, Christian and inter-community profiles waiting to begin their lifelong journey with you.',
      'hero.lookingFor': "I'm looking for a",
      'hero.bride': 'Bride (Female)',
      'hero.groom': 'Groom (Male)',
      'hero.age': 'Age',
      'hero.religion': 'Religion',
      'hero.motherTongue': 'Mother Tongue',
      'hero.searchBtn': "Let's Find Matches",

      'common.verified': '100% Verified',
      'common.sendInterest': 'Send Interest',
      'common.interestSent': 'Interest Sent',
      'common.shortlist': 'Shortlist',
      'common.shortlisted': 'Shortlisted',
      'common.viewDetails': 'View Details',
      'common.chatNow': 'Chat Now',
      'common.callNow': 'Call Now',
      'common.accept': 'Accept',
      'common.decline': 'Decline'
    },
    hi: {
      'nav.home': 'होम',
      'nav.matches': 'मैचेस',
      'nav.interests': 'रुचियाँ (Interests)',
      'nav.chat': 'चैट',
      'nav.shortlist': 'शॉर्टलिस्ट',
      'nav.search': 'सर्च',
      'nav.preferences': 'पार्टनर पसंद',
      'nav.profile': 'मेरी प्रोफाइल',
      'nav.login': 'लॉगिन',
      'nav.registerFree': 'मुफ्त पंजीकरण',
      'nav.logout': 'लॉगआउट',
      'nav.language': 'भाषा',

      'hero.badge': '✨ भारत का सबसे भरोसेमंद विवाह संगम',
      'hero.titlePart1': 'अपना सही',
      'hero.titleHighlight': 'जीवनसाथी खोजें',
      'hero.subtitle': '5,00,000 से अधिक सत्यापित प्रोफाइल में से अपने सपनों का जीवनसाथी खोजें और पवित्र बंधन की शुरुआत करें।',
      'hero.lookingFor': 'मैं तलाश रहा हूँ',
      'hero.bride': 'दुल्हन (वधू)',
      'hero.groom': 'दूल्हा (वर)',
      'hero.age': 'उम्र',
      'hero.religion': 'धर्म',
      'hero.motherTongue': 'मातृभाषा',
      'hero.searchBtn': 'जीवनसाथी खोजें',

      'common.verified': '100% सत्यापित',
      'common.sendInterest': 'रुचि भेजें',
      'common.interestSent': 'रुचि भेजी गई',
      'common.shortlist': 'शॉर्टलिस्ट',
      'common.shortlisted': 'शॉर्टलिस्टेड',
      'common.viewDetails': 'विवरण देखें',
      'common.chatNow': 'चैट करें',
      'common.callNow': 'कॉल करें',
      'common.accept': 'स्वीकार करें',
      'common.decline': 'अस्वीकार करें'
    },
    bn: {
      'nav.home': 'হোম',
      'nav.matches': 'ম্যাচ',
      'nav.interests': 'আগ্রহ',
      'nav.chat': 'চ্যাট',
      'nav.shortlist': 'শর্টলিস্ট',
      'nav.search': 'অনুসন্ধান',
      'nav.preferences': 'পছন্দ',
      'nav.profile': 'প্রোফাইল',
      'nav.login': 'লগইন',
      'nav.registerFree': 'বিনামূল্যে নিবন্ধন',
      'nav.logout': 'লগআউট',
      'nav.language': 'ভাষা',

      'hero.badge': '✨ ভারতের সবচেয়ে বিশ্বস্ত বৈবাহিক প্ল্যাটফর্ম',
      'hero.titlePart1': 'আপনার আদর্শ',
      'hero.titleHighlight': 'জীবনসঙ্গী খুঁজুন',
      'hero.subtitle': 'লক্ষাধিক যাচাইকৃত পাত্র-পাত্রীর প্রোফাইল থেকে আপনার পছন্দের মানুষটিকে বেছে নিন।',
      'hero.lookingFor': 'আমি খুঁজছি',
      'hero.bride': 'পাত্রী (মেয়ে)',
      'hero.groom': 'পাত্র (ছেলে)',
      'hero.age': 'বয়স',
      'hero.religion': 'ধর্ম',
      'hero.motherTongue': 'মাতৃভাষা',
      'hero.searchBtn': 'সন্ধান করুন',

      'common.verified': '১০০% যাচাইকৃত',
      'common.sendInterest': 'আগ্রহ পাঠান',
      'common.interestSent': 'আগ্রহ পাঠানো হয়েছে',
      'common.shortlist': 'শর্টলিস্ট',
      'common.shortlisted': 'সংরক্ষিত',
      'common.viewDetails': 'বিস্তারিত দেখুন',
      'common.chatNow': 'চ্যাট করুন',
      'common.callNow': 'কল করুন',
      'common.accept': 'গ্রহণ করুন',
      'common.decline': 'প্রত্যাখ্যান করুন'
    },
    mr: {
      'nav.home': 'मुख्यपृष्ठ',
      'nav.matches': 'अनुरूप स्थळे',
      'nav.interests': 'पसंती (Interests)',
      'nav.chat': 'संवाद (Chat)',
      'nav.shortlist': 'जतन स्थळे',
      'nav.search': 'शोध',
      'nav.preferences': 'अपेक्षा',
      'nav.profile': 'माझे प्रोफाईल',
      'nav.login': 'लॉगिन',
      'nav.registerFree': 'मोफत नोंदणी',
      'nav.logout': 'बाहेर पडा',
      'nav.language': 'भाषा',

      'hero.badge': '✨ महाराष्ट्रातील सर्वात विश्वासू विवाह व्यासपीठ',
      'hero.titlePart1': 'आपला योग्य',
      'hero.titleHighlight': 'जीवनसाथी शोधा',
      'hero.subtitle': 'लाखो पडताळणी झालेल्या स्थळांमधून आपल्या आवडीचा जीवनसाथी निवडा आणि नवीन जीवन सुरू करा.',
      'hero.lookingFor': 'मला पाहिजे',
      'hero.bride': 'वधू (मुलगी)',
      'hero.groom': 'वर (मुलगा)',
      'hero.age': 'वय',
      'hero.religion': 'धर्म',
      'hero.motherTongue': 'मातृभाषा',
      'hero.searchBtn': 'स्थळे शोधा',

      'common.verified': '१००% पडताळणीकृत',
      'common.sendInterest': 'पसंती पाठवा',
      'common.interestSent': 'पसंती पाठवली',
      'common.shortlist': 'जतन करा',
      'common.shortlisted': 'जतन केले',
      'common.viewDetails': 'माहिती पहा',
      'common.chatNow': 'चॅट करा',
      'common.callNow': 'कॉल करा',
      'common.accept': 'स्वीकारा',
      'common.decline': 'नकारा'
    },
    ta: {
      'nav.home': 'முகப்பு',
      'nav.matches': 'பொருத்தங்கள்',
      'nav.interests': 'விருப்பங்கள்',
      'nav.chat': 'உரையாடல்',
      'nav.shortlist': 'தேர்ந்தெடுத்தவை',
      'nav.search': 'தேடல்',
      'nav.preferences': 'விருப்பத்தேர்வு',
      'nav.profile': 'சுயவிவரம்',
      'nav.login': 'உள்நுழை',
      'nav.registerFree': 'இலவச பதிவு',
      'nav.logout': 'வெளியேறு',
      'nav.language': 'மொழி',

      'hero.badge': '✨ இந்தியாவின் நம்பகமான திருமண மேடை',
      'hero.titlePart1': 'உங்கள் சிறந்த',
      'hero.titleHighlight': 'வாழ்க்கைத் துணையைத் தேடுங்கள்',
      'hero.subtitle': 'நம்பகமான வரன்களைத் தேர்ந்தெடுத்து உங்கள் திருமண வாழ்க்கையைத் தொடங்குங்கள்.',
      'hero.lookingFor': 'நான் தேடுவது',
      'hero.bride': 'மணமகள் (பெண்)',
      'hero.groom': 'மணமகன் (ஆண்)',
      'hero.age': 'வயது',
      'hero.religion': 'மதம்',
      'hero.motherTongue': 'தாய்மொழி',
      'hero.searchBtn': 'வரன் தேடுக',

      'common.verified': '100% சரிபார்க்கப்பட்டது',
      'common.sendInterest': 'விருப்பம் அனுப்பு',
      'common.interestSent': 'விருப்பம் அனுப்பப்பட்டது',
      'common.shortlist': 'பட்டியலிடு',
      'common.shortlisted': 'பட்டியலிடப்பட்டது',
      'common.viewDetails': 'விவரம் காண்க',
      'common.chatNow': 'உரையாடு',
      'common.callNow': 'அழைக்க',
      'common.accept': 'ஏற்றுக்கொள்',
      'common.decline': 'நிராகரி'
    },
    te: {
      'nav.home': 'హోమ్',
      'nav.matches': 'జంటలు',
      'nav.interests': 'ఆసక్తులు',
      'nav.chat': 'చాట్',
      'nav.shortlist': 'షార్ట్‌లిస్ట్',
      'nav.search': 'వెతకండి',
      'nav.preferences': 'ప్రాధాన్యతలు',
      'nav.profile': 'ప్రొఫైల్',
      'nav.login': 'లాగిన్',
      'nav.registerFree': 'ఉచిత నమోదు',
      'nav.logout': 'లాగౌట్',
      'nav.language': 'భాష',

      'hero.badge': '✨ అత్యంత విశ్వసనీయ వైవాహిక వేదిక',
      'hero.titlePart1': 'మీ సరైన',
      'hero.titleHighlight': 'జీవిత భాగస్వామిని ఎంచుకోండి',
      'hero.subtitle': 'లక్షలాది సరిచూసిన ప్రొఫైల్స్ నుండి మీ జీవిత భాగస్వామిని సులభంగా కనుగొనండి.',
      'hero.lookingFor': 'నేను చూస్తున్నది',
      'hero.bride': 'వధువు (అమ్మాయి)',
      'hero.groom': 'వరుడు (అబ్బాయి)',
      'hero.age': 'వయస్సు',
      'hero.religion': 'మతం',
      'hero.motherTongue': 'మాతృభాష',
      'hero.searchBtn': 'వెతకండి',

      'common.verified': '100% ధృవీకరించబడింది',
      'common.sendInterest': 'ఆసక్తి తెలపండి',
      'common.interestSent': 'ఆసక్తి పంపబడింది',
      'common.shortlist': 'షార్ట్‌లిస్ట్',
      'common.shortlisted': 'సేవ్ చేయబడింది',
      'common.viewDetails': 'వివరాలు చూడండి',
      'common.chatNow': 'చాట్ చేయండి',
      'common.callNow': 'కాల్ చేయండి',
      'common.accept': 'అంగీకరించు',
      'common.decline': 'తిరస్కరించు'
    },
    gu: {
      'nav.home': 'હોમ',
      'nav.matches': 'મેચિસ',
      'nav.interests': 'પસંદગી',
      'nav.chat': 'વાતચીત',
      'nav.shortlist': 'શોર્ટલિસ્ટ',
      'nav.search': 'શોધ',
      'nav.preferences': 'જીવનસાથી પસંદગી',
      'nav.profile': 'પ્રોફાઇલ',
      'nav.login': 'લૉગિન',
      'nav.registerFree': 'મફત રજીસ્ટ્રેશન',
      'nav.logout': 'લૉગઆઉટ',
      'nav.language': 'ભાષા',

      'hero.badge': '✨ ભારતનું સૌથી વિશ્વસનીય મેટ્રિમોનિયલ પ્લેટફોર્મ',
      'hero.titlePart1': 'તમારો યોગ્ય',
      'hero.titleHighlight': 'જીવનસાથી શોધો',
      'hero.subtitle': 'લાખો વેરિફાઇડ પ્રોફાઇલમાંથી તમારા સપનાનો સાથી શોધો અને સુખી જીવન શરૂ કરો.',
      'hero.lookingFor': 'હું શોધી રહ્યો છું',
      'hero.bride': 'કન્યા (વહુ)',
      'hero.groom': 'વર (છોકરો)',
      'hero.age': 'ઉંમર',
      'hero.religion': 'ધર્મ',
      'hero.motherTongue': 'માતૃભાષા',
      'hero.searchBtn': 'મેચ શોધો',

      'common.verified': '૧૦૦% વેરિફાઇડ',
      'common.sendInterest': 'રસ દાખવો',
      'common.interestSent': 'રસ મોકલ્યો',
      'common.shortlist': 'શોર્ટલિસ્ટ કરો',
      'common.shortlisted': 'શોર્ટલિસ્ટ થયું',
      'common.viewDetails': 'વિગતો જુઓ',
      'common.chatNow': 'ચેટ કરો',
      'common.callNow': 'કૉલ કરો',
      'common.accept': 'સ્વીકારો',
      'common.decline': 'નકારો'
    },
    kn: {
      'nav.home': 'ಮುಖಪುಟ',
      'nav.matches': 'ಹೊಂದಾಣಿಕೆಗಳು',
      'nav.interests': 'ಆಸಕ್ತಿಗಳು',
      'nav.chat': 'ಸಂವಾದ',
      'nav.shortlist': 'ಆಯ್ಕೆಪಟ್ಟಿ',
      'nav.search': 'ಹುಡುಕಿ',
      'nav.preferences': 'ಆದ್ಯತೆಗಳು',
      'nav.profile': 'ಪ್ರೊಫೈಲ್',
      'nav.login': 'ಲಾಗಿನ್',
      'nav.registerFree': 'ಉಚಿತ ನೋಂದಣಿ',
      'nav.logout': 'ನಿರ್ಗಮಿಸಿ',
      'nav.language': 'ಭಾಷೆ',

      'hero.badge': '✨ ಭಾರತದ ಅತ್ಯಂತ ವಿಶ್ವಾಸಾರ್ಹ ವೈವಾಹಿಕ ತಾಣ',
      'hero.titlePart1': 'ನಿಮ್ಮ ಸೂಕ್ತ',
      'hero.titleHighlight': 'ಜೀವನ ಸಂಗಾತಿಯನ್ನು ಹುಡುಕಿ',
      'hero.subtitle': 'ಲಕ್ಷಾಂತರ ಪರಿಶೀಲಿಸಿದ ಪ್ರೊಫೈಲ್‌ಗಳಿಂದ ನಿಮ್ಮ ಮೆಚ್ಚಿನ ಬಾಳಸಂಗಾತಿಯನ್ನು ಆರಿಸಿ.',
      'hero.lookingFor': 'ನಾನು ಹುಡುಕುತ್ತಿರುವುದು',
      'hero.bride': 'ವಧು (ಹೆಣ್ಣು)',
      'hero.groom': 'ವರ (ಗಂಡು)',
      'hero.age': 'ವಯಸ್ಸು',
      'hero.religion': 'ಧರ್ಮ',
      'hero.motherTongue': 'ಮಾತೃಭಾಷೆ',
      'hero.searchBtn': 'ಹುಡುಕಿ',

      'common.verified': '100% ಪರಿಶೀಲಿಸಲಾಗಿದೆ',
      'common.sendInterest': 'ಆಸಕ್ತಿ ಕಳುಹಿಸಿ',
      'common.interestSent': 'ಆಸಕ್ತಿ ಕಳುಹಿಸಲಾಗಿದೆ',
      'common.shortlist': 'ಆಯ್ಕೆ ಮಾಡಿ',
      'common.shortlisted': 'ಆಯ್ಕೆಮಾಡಲಾಗಿದೆ',
      'common.viewDetails': 'ವಿವರ ನೋಡಿ',
      'common.chatNow': 'ಚಾಟ್ ಮಾಡಿ',
      'common.callNow': 'ಕರೆ ಮಾಡಿ',
      'common.accept': 'ಸ್ವೀಕರಿಸಿ',
      'common.decline': 'ನಿರಾಕರಿಸಿ'
    }
  };

  constructor(private http: HttpClient) {}

  public get currentLanguage(): string {
    return this.currentLangSubject.value;
  }

  public get currentLanguageOption(): LanguageOption {
    return this.supportedLanguages.find(l => l.code === this.currentLanguage) || this.supportedLanguages[0];
  }

  public setLanguage(code: string): void {
    if (this.translations[code]) {
      this.currentLangSubject.next(code);
      localStorage.setItem('milansetu_lang', code);

      // Persist to backend if token exists
      const token = localStorage.getItem('milansetu_token');
      if (token) {
        this.http.put(this.apiUrl, { language: code }).subscribe({
          next: () => {},
          error: () => {}
        });
      }
    }
  }

  public translate(key: string): string {
    const lang = this.currentLanguage;
    const dict = this.translations[lang] || this.translations['en'];
    return dict[key] || this.translations['en'][key] || key;
  }

  private getStoredLanguage(): string {
    return localStorage.getItem('milansetu_lang') || 'en';
  }
}
