# 💍 MilanSetu Matrimony — Complete System Workflow & Architecture Manual

> **MilanSetu** is an enterprise-grade Indian Matrimony Web Application. This document provides end-to-end workflow diagrams, database architecture, communication lifecycle, and step-by-step feature execution flows.

---

## 📑 Table of Workflows
1. [High-Level System Architecture](#1-high-level-system-architecture)
2. [User Registration & Profile Initialization Flow](#2-user-registration--profile-initialization-flow)
3. [Authentication, Google OAuth & OTP Password Recovery Flow](#3-authentication-google-oauth--otp-password-recovery-flow)
4. [7-Factor Compatibility Matching Algorithm Workflow](#4-7-factor-compatibility-matching-algorithm-workflow)
5. [Express Interest, Shortlist & Mutual Match Flow](#5-express-interest-shortlist--mutual-match-flow)
6. [Real-time SignalR Chat & Messaging Flow](#6-real-time-signalr-chat--messaging-flow)
7. [Profile View & Visitor Notification Workflow](#7-profile-view--visitor-notification-workflow)
8. [Admin Control Center & Governance Workflow](#8-admin-control-center--governance-workflow)
9. [Runtime Multi-Language Localization Workflow](#9-runtime-multi-language-localization-workflow)
10. [Complete Database Schema & Relational Entity Flow](#10-complete-database-schema--relational-entity-flow)

---

## 1. High-Level System Architecture

```mermaid
graph TB
    subgraph Frontend ["Angular 19 Standalone Frontend (Port 4200)"]
        UI["UI Components<br>(Home, Matches, Chat, Search, Admin, Profile)"]
        Services["Angular Services<br>(Auth, Matches, Chat, Admin, MasterData)"]
        SignalRClient["SignalR Hub Client<br>(@microsoft/signalr)"]
        I18n["Translation Engine<br>(8 Indic Languages)"]
    end

    subgraph Backend [".NET 8 Web API (Port 5000 / 5056)"]
        Controllers["12 REST API Controllers<br>(Auth, Matches, Admin, Chat, Search, etc.)"]
        Hubs["SignalR Chat Hub<br>(/hubs/chat)"]
        MatchingEngine["7-Factor Matching Engine<br>(IMatchingService)"]
        EmailService["SMTP / HTML Email Dispatcher"]
        EF["Entity Framework Core 8<br>& Stored Procedure Executor"]
    end

    subgraph Database ["Microsoft SQL Server (MilanSetuDB)"]
        CoreTables["11 Core Tables<br>(Users, Profiles, Preferences, Chat, Interests)"]
        MasterTables["6 Master Tables<br>(Religions, Languages, Occupations, Cities)"]
        StoredProcs["13 Stored Procedures<br>(sp_RegisterUser, sp_Search, sp_Chat, etc.)"]
    end

    UI --> Services
    UI <--> SignalRClient
    Services -->|"REST API Calls (JWT Bearer)"| Controllers
    SignalRClient <-->|"WebSockets / Long Polling"| Hubs
    Controllers --> MatchingEngine
    Controllers --> EmailService
    Controllers --> EF
    Hubs --> EF
    EF <--> CoreTables
    EF <--> MasterTables
    EF <--> StoredProcs
```

---

## 2. User Registration & Profile Initialization Flow

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant Angular as Angular 19 UI
    participant API as .NET 8 Web API
    participant SQL as SQL Server (MilanSetuDB)

    User->>Angular: Enters Basic Info (Name, Gender, DOB, Email, Mobile, Password)
    User->>Angular: Selects Religion, Caste & Mother Tongue (from Master Tables)
    User->>Angular: Enters Living Location (City, State)
    User->>Angular: Clicks "Register Free"
    Angular->>API: POST /api/auth/register
    API->>API: Hashes Password with BCrypt (Cost Factor 11)
    API->>SQL: Execute sp_RegisterUser
    SQL->>SQL: INSERT INTO [dbo].[Users]
    SQL->>SQL: Initialize Default [dbo].[UserProfiles] (Completion 65%)
    SQL->>SQL: Initialize Default [dbo].[PartnerPreferences]
    SQL-->>API: Returns New UserId (e.g. MS-1042)
    API->>API: Generates JWT Bearer Token (24-Hour Expiry)
    API-->>Angular: HTTP 200 { Token, UserObject, Message }
    Angular->>User: Displays SweetAlert2 Celebration Screen & Redirects to Profile Setup
```

---

## 3. Authentication, Google OAuth & OTP Password Recovery Flow

### A. Google OAuth / Fast Sign-In Flow:
```mermaid
graph LR
    A["User clicks 'Continue with Google'"] --> B["Interactive Google Prompt / GIS Dialog"]
    B --> C["User enters real Gmail & Name"]
    C --> D["POST /api/auth/google-login"]
    D --> E{"Account exists in SQL?"}
    E -- No --> F["Create User + Profile Defaults in SQL"]
    E -- Yes --> G["Retrieve Existing Account"]
    F --> H["Issue JWT Token & Authenticate Session"]
    G --> H
    H --> I["Redirect to Matches Dashboard"]
```

### B. Forgot Password (6-Digit Email OTP) Flow:
```mermaid
graph TD
    A["User clicks 'Forgot Password'"] --> B["Enters registered Email or Mobile"]
    B --> C["POST /api/auth/forgot-password"]
    C --> D["Backend verifies user existence in SQL"]
    D --> E["Generate cryptographically secure 6-digit OTP"]
    E --> F["INSERT into PasswordResetOtps (10 Min Expiry)"]
    F --> G["HTML Email Dispatched via EmailService"]
    F --> H["OTP displayed on screen modal in Dev mode"]
    G & H --> I["User enters 6-digit OTP + New Password"]
    I --> J["POST /api/auth/reset-password"]
    J --> K["Validate OTP & Update BCrypt Password in Users Table"]
    K --> L["SweetAlert2 Success: Log in with New Password"]
```

---

## 4. 7-Factor Compatibility Matching Algorithm Workflow

The platform matches registered candidates based on a weighted 100-point algorithm:

```mermaid
pie title MilanSetu 100-Point Compatibility Score Weightage
    "Age Preference (±3 Years)" : 20
    "Religion & Community Alignment" : 20
    "Mother Tongue / Language" : 15
    "Education Qualification Level" : 15
    "Location Proximity (City/State)" : 10
    "Annual Income Tier Compatibility" : 10
    "Diet & Lifestyle Match" : 10
```

### Matching Algorithm Pipeline:
$$\text{Total Score} = S_{\text{Age}} + S_{\text{Religion}} + S_{\text{MotherTongue}} + S_{\text{Education}} + S_{\text{Location}} + S_{\text{Income}} + S_{\text{Lifestyle}}$$

- **90% - 100%**: 🌟 **Elite / Perfect Match**
- **75% - 89%**: ✨ **Highly Compatible**
- **60% - 74%**: 👍 **Good Match**
- **Below 60%**: 🔍 **Partial Match**

---

## 5. Express Interest, Shortlist & Mutual Match Flow

```mermaid
sequenceDiagram
    autonumber
    actor CandidateA as Rahul (Sender)
    participant Portal as MilanSetu Web UI
    participant API as Backend API
    participant SQL as SQL Server
    actor CandidateB as Priya (Receiver)

    CandidateA->>Portal: Clicks "Send Interest ❤️" on Priya's Card
    Portal->>API: POST /api/interests/send { senderId, receiverId }
    API->>SQL: Execute sp_SendInterest
    SQL->>SQL: INSERT INTO [dbo].[UserInterests] (Status = 'Pending')
    SQL->>SQL: INSERT INTO [dbo].[Notifications] (Type = 'Interest', UserId = Priya)
    SQL-->>API: Success
    API-->>Portal: Toast: "Interest Sent Successfully!"

    Note over CandidateB,Portal: Priya opens MilanSetu or checks Notification Bell
    CandidateB->>Portal: Priya sees: "Rahul (MS-1042) sent you an Interest ❤️"
    CandidateB->>Portal: Priya clicks [Accept Interest ✓]
    Portal->>API: POST /api/interests/respond { status: 'Accepted' }
    API->>SQL: Execute sp_RespondInterest
    SQL->>SQL: UPDATE [dbo].[UserInterests] SET Status = 'Accepted'
    SQL->>SQL: INSERT Notification for Rahul: "Priya accepted your interest! 🎉"
    Note over CandidateA,CandidateB: 💬 Direct 1-on-1 Chat and Contact Details are Unlocked!
```

---

## 6. Real-time SignalR Chat & Messaging Flow

```mermaid
sequenceDiagram
    autonumber
    actor User1 as User A (Browser 1)
    participant Hub as SignalR ChatHub (/hubs/chat)
    participant SQL as SQL Server (ChatMessages Table)
    actor User2 as User B (Browser 2)

    User1->>Hub: Connects via WebSocket (/hubs/chat)
    User2->>Hub: Connects via WebSocket (/hubs/chat)
    Hub->>Hub: Registers User Connection IDs

    User1->>Hub: Invokes SendPrivateMessage(ReceiverId, Content)
    Hub->>SQL: Execute sp_SendMessage (Inserts message into ChatMessages)
    Hub->>User2: Broadcasts "ReceiveMessage" Event in real-time (< 50ms)
    Hub-->>User1: Acknowledges message sent
    User2->>User1: Types response... (Triggers "UserTyping" indicator)
    User2->>Hub: Marks Message as Read
    Hub->>SQL: UPDATE ChatMessages SET IsRead = 1, ReadAt = SYSUTCDATETIME()
```

---

## 7. Profile View & Visitor Notification Workflow

```mermaid
graph TD
    A["User A views User B's Matrimonial Profile"] --> B["POST /api/interests/record-view"]
    B --> C["Execute sp_RecordProfileView in SQL"]
    C --> D["INSERT INTO [dbo].[ProfileViews] (ViewerId, ViewedId, ViewedAt)"]
    D --> E["INSERT INTO [dbo].[Notifications]<br>Title: '👀 Profile Viewed!'<br>Message: 'Member MS-1042 (Rahul Sharma) just viewed your profile.'"]
    E --> F["User B's Notification Bell shows unread badge counter"]
    F --> G["User B clicks notification to view Rahul's profile & match score"]
```

---

## 8. Admin Control Center & Governance Workflow

```mermaid
graph TB
    subgraph AdminActions ["Admin Portal Controls (/admin)"]
        Analytics["📊 Live KPI Analytics<br>(Total Members, Verified %, Messages, Matches)"]
        MemberMgmt["👥 Member Directory & Verification<br>(Approve Badge, Block/Unblock, Delete)"]
        Queue["🛡️ Verification Queue<br>(Review pending photos & documents)"]
        StoriesMgmt["💍 Success Stories Publisher<br>(Add new couple photos, quotes & publish)"]
        MasterControl["🗄️ Master Data Manager<br>(Add Religions, Languages, Cities to SQL)"]
    end

    subgraph DatabaseEffect ["Database Synchronization"]
        UsersTable["[dbo].[Users] Update (IsVerified, IsBlocked)"]
        StoriesTable["[dbo].[SuccessStories] (Live Homepage Carousel)"]
        MasterDB["[dbo].[MasterReligions], [dbo].[MasterLocations]"]
    end

    MemberMgmt --> UsersTable
    Queue --> UsersTable
    StoriesMgmt --> StoriesTable
    MasterControl --> MasterDB
```

---

## 9. Runtime Multi-Language Localization Workflow

```mermaid
graph LR
    A["User clicks Language Dropdown in Navbar"] --> B["Selects Language (e.g. 🇮🇳 Hindi, বাংলা, मराठी, தமிழ், etc.)"]
    B --> C["TranslationService.setLanguage(langCode)"]
    C --> D["Updates active language dictionary & localStorage"]
    D --> E["Angular TranslatePipe triggers instant re-render across all UI components"]
    E --> F["Labels, Menus, Placeholders & Buttons change without page reload"]
```

---

## 10. Complete Database Schema & Relational Entity Flow

```mermaid
erDiagram
    Users ||--o{ UserProfiles : "1:1 profile attributes"
    Users ||--o{ PartnerPreferences : "1:1 desired partner criteria"
    Users ||--o{ UserInterests : "sends/receives connects"
    Users ||--o{ UserShortlists : "bookmarks favorites"
    Users ||--o{ ChatMessages : "sends/receives messages"
    Users ||--o{ Notifications : "receives alerts"
    Users ||--o{ ProfileViews : "tracks visitors"

    Users {
        int Id PK "Matrimony ID (MS-xxx)"
        string Name "Full Candidate Name"
        string Gender "'Male' or 'Female'"
        datetime DateOfBirth "DOB for Age Calculation"
        string Email UK "Login Identifier"
        string Mobile UK "Contact Number"
        string PasswordHash "BCrypt Hashed"
        string Religion "Primary Faith"
        string Caste "Community / Sub-caste"
        string MotherTongue "Native Language"
        string Location "City, State, Country"
        string ProfilePhotoUrl "Display Avatar"
        bool IsVerified "Verified Badge Status"
        string Role "'User' or 'Admin'"
        bool IsBlocked "Account Ban Status"
        string PreferredLanguage "'en', 'hi', etc."
        datetime CreatedAt "Registration Timestamp"
    }

    UserProfiles {
        int Id PK
        int UserId FK
        string Height "e.g. 5'8''"
        string MaritalStatus "'Never Married', etc."
        string HighestEducation "B.Tech, MBA, MBBS"
        string EmployedIn "Private, Govt, Business"
        string Occupation "Software Architect, Doctor"
        string AnnualIncome "₹15 - ₹25 Lakhs"
        string FamilyType "Nuclear / Joint"
        string Diet "Vegetarian / Non-Veg"
        string ManglikStatus "'Yes', 'No', 'Partial'"
        string City "Living City"
        string State "Living State"
        int ProfileCompletionPercentage "65% - 100%"
    }

    PartnerPreferences {
        int Id PK
        int UserId FK
        int MinAge "Min Age Preference"
        int MaxAge "Max Age Preference"
        string Religion "Preferred Faith"
        string MotherTongue "Preferred Language"
        string Education "Preferred Qualification"
        string MinAnnualIncome "Income Benchmark"
    }

    UserInterests {
        int Id PK
        int SenderUserId FK
        int ReceiverUserId FK
        string Status "'Pending', 'Accepted', 'Declined'"
        string CustomMessage "Intro Message"
        datetime SentAt "Dispatch Time"
        datetime RespondedAt "Action Time"
    }

    ChatMessages {
        int Id PK
        int SenderId FK
        int ReceiverId FK
        string Content "Encrypted Message Text"
        bool IsRead "Read Receipt"
        datetime SentAt "Message Timestamp"
    }

    SuccessStories {
        int Id PK
        string CoupleName "e.g. Vikram & Radhika"
        string WeddingDate "e.g. December 2025"
        string Location "Destination City"
        string ImageUrl "High-res Couple Picture"
        string Quote "Review Testimonial"
        string StorySnippet "Love Story Summary"
        bool IsFeatured "Homepage Showcase"
    }

    MasterReligions {
        int Id PK
        string Name "Faith Name"
        int SortOrder "Display Sequence"
        bool IsActive "Active Flag"
    }

    MasterLocations {
        int Id PK
        string CityName "City"
        string StateName "State"
        string Country "Default: India"
        bool IsPopular "Top Tier City"
        int SortOrder "Display Sequence"
    }
```

---

## 11. Summary of Execution Commands

| Component | Command | Port / URL |
| :--- | :--- | :--- |
| **Backend API** | `dotnet run --project Backend/MilanSetu.API/MilanSetu.API` | `http://localhost:5000` |
| **Frontend UI** | `npm start` (inside `Frontend/MilanSetu.UI`) | `http://localhost:4200` |
| **Admin Portal** | Access via browser | `http://localhost:4200/admin` |
| **Database Script** | Execute in SSMS | `Database/00_Full_MilanSetu_Master_Script.sql` |
