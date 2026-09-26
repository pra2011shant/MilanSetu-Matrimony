-- ============================================================================
-- Project: MilanSetu Matrimony Platform
-- Database Creation, Tables Schema, Foreign Keys, and Performance Indexes
-- Engine: Microsoft SQL Server 2019 / 2022 / Azure SQL
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MilanSetuDB')
BEGIN
    CREATE DATABASE [MilanSetuDB];
END
GO

USE [MilanSetuDB];
GO

-- ----------------------------------------------------------------------------
-- 1. TABLE: Users
-- Description: Core authentication and primary user account information
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Gender NVARCHAR(20) NOT NULL,
        DateOfBirth DATETIME2(7) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        Mobile NVARCHAR(20) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Religion NVARCHAR(50) NOT NULL,
        Caste NVARCHAR(100) NULL,
        MotherTongue NVARCHAR(50) NOT NULL,
        Location NVARCHAR(150) NOT NULL,
        ProfilePhotoUrl NVARCHAR(MAX) NULL,
        IsVerified BIT NOT NULL CONSTRAINT DF_Users_IsVerified DEFAULT (1),
        Role NVARCHAR(20) NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('User'),
        IsBlocked BIT NOT NULL CONSTRAINT DF_Users_IsBlocked DEFAULT (0),
        PreferredLanguage NVARCHAR(10) NOT NULL CONSTRAINT DF_Users_PreferredLanguage DEFAULT ('en'),
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 2. TABLE: UserProfiles
-- Description: Detailed 11-section matrimonial profile of the user
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfiles (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        -- 1. Basic Info
        Height NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Height DEFAULT ('5''7"'),
        Weight NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Weight DEFAULT ('65 kg'),
        MaritalStatus NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_MaritalStatus DEFAULT ('Never Married'),
        PhysicalStatus NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_PhysicalStatus DEFAULT ('Normal'),
        ProfileManagedBy NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_ProfileManagedBy DEFAULT ('Self'),
        -- 2. About Me
        AboutMe NVARCHAR(1000) NULL,
        PartnerExpectations NVARCHAR(500) NULL,
        -- 3. Education
        HighestEducation NVARCHAR(100) NULL,
        CollegeOrUniversity NVARCHAR(150) NULL,
        FieldOfStudy NVARCHAR(100) NULL,
        -- 4. Profession
        EmployedIn NVARCHAR(50) NULL CONSTRAINT DF_UserProfiles_EmployedIn DEFAULT ('Private Sector'),
        Occupation NVARCHAR(100) NULL,
        CompanyName NVARCHAR(150) NULL,
        WorkLocation NVARCHAR(100) NULL,
        -- 5. Income
        AnnualIncome NVARCHAR(50) NULL CONSTRAINT DF_UserProfiles_AnnualIncome DEFAULT (N'₹10 - ₹15 Lakhs'),
        -- 6. Family Details
        FamilyType NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_FamilyType DEFAULT ('Nuclear'),
        FamilyValues NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_FamilyValues DEFAULT ('Moderate'),
        FatherOccupation NVARCHAR(100) NULL,
        MotherOccupation NVARCHAR(100) NULL,
        NumberOfBrothers INT NOT NULL CONSTRAINT DF_UserProfiles_Brothers DEFAULT (0),
        NumberOfSisters INT NOT NULL CONSTRAINT DF_UserProfiles_Sisters DEFAULT (0),
        FamilyCity NVARCHAR(150) NULL,
        -- 7. Lifestyle
        Diet NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Diet DEFAULT ('Vegetarian'),
        Drink NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Drink DEFAULT ('No'),
        Smoke NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Smoke DEFAULT ('No'),
        -- 8. Hobbies
        Hobbies NVARCHAR(300) NULL CONSTRAINT DF_UserProfiles_Hobbies DEFAULT ('Travelling, Reading, Music, Fitness'),
        -- 9. Religion & Astrology
        SubCasteOrGothra NVARCHAR(100) NULL,
        ManglikStatus NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Manglik DEFAULT ('No'),
        Rashi NVARCHAR(50) NULL,
        Nakshatra NVARCHAR(50) NULL,
        -- 10. Location
        City NVARCHAR(100) NULL,
        State NVARCHAR(100) NULL,
        Country NVARCHAR(100) NULL CONSTRAINT DF_UserProfiles_Country DEFAULT ('India'),
        NativePlace NVARCHAR(100) NULL,
        WillingToRelocate BIT NOT NULL CONSTRAINT DF_UserProfiles_Relocate DEFAULT (1),
        -- 11. Photos & Metas
        PhotoGalleryJson NVARCHAR(MAX) NULL,
        ProfileCompletionPercentage INT NOT NULL CONSTRAINT DF_UserProfiles_Completion DEFAULT (85),
        UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_UserProfiles_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserProfiles PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_UserProfiles_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- 3. TABLE: PartnerPreferences
-- Description: Partner search criteria used by matchmaking recommendation engine
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.PartnerPreferences', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerPreferences (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        MinAge INT NOT NULL CONSTRAINT DF_PartnerPreferences_MinAge DEFAULT (21),
        MaxAge INT NOT NULL CONSTRAINT DF_PartnerPreferences_MaxAge DEFAULT (30),
        MinHeight NVARCHAR(20) NOT NULL CONSTRAINT DF_PartnerPreferences_MinHeight DEFAULT ('5''0"'),
        MaxHeight NVARCHAR(20) NOT NULL CONSTRAINT DF_PartnerPreferences_MaxHeight DEFAULT ('5''10"'),
        Religion NVARCHAR(100) NOT NULL CONSTRAINT DF_PartnerPreferences_Religion DEFAULT ('Any Religion'),
        Community NVARCHAR(150) NOT NULL CONSTRAINT DF_PartnerPreferences_Community DEFAULT ('Open to All / Caste No Bar'),
        MotherTongue NVARCHAR(100) NOT NULL CONSTRAINT DF_PartnerPreferences_MotherTongue DEFAULT ('Any Language'),
        Education NVARCHAR(150) NOT NULL CONSTRAINT DF_PartnerPreferences_Education DEFAULT ('Graduate / Post Graduate & Above'),
        Profession NVARCHAR(150) NOT NULL CONSTRAINT DF_PartnerPreferences_Profession DEFAULT ('Private / Govt / Business Professional'),
        MinAnnualIncome NVARCHAR(50) NOT NULL CONSTRAINT DF_PartnerPreferences_MinIncome DEFAULT (N'₹5 Lakhs & Above'),
        PreferredLocation NVARCHAR(200) NOT NULL CONSTRAINT DF_PartnerPreferences_Location DEFAULT ('Any Location in India / Open to Relocate'),
        MaritalStatus NVARCHAR(50) NOT NULL CONSTRAINT DF_PartnerPreferences_MaritalStatus DEFAULT ('Never Married'),
        Diet NVARCHAR(50) NOT NULL CONSTRAINT DF_PartnerPreferences_Diet DEFAULT ('Vegetarian / Eggetarian'),
        Drink NVARCHAR(50) NOT NULL CONSTRAINT DF_PartnerPreferences_Drink DEFAULT ('Non-Drinker / Social Drinker'),
        Smoke NVARCHAR(50) NOT NULL CONSTRAINT DF_PartnerPreferences_Smoke DEFAULT ('Non-Smoker'),
        UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_PartnerPreferences_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PartnerPreferences PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_PartnerPreferences_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- 4. TABLE: UserInterests (Express Interest)
-- Description: Connect requests, status (Pending, Accepted, Declined)
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.UserInterests', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserInterests (
        Id INT IDENTITY(1,1) NOT NULL,
        SenderUserId INT NOT NULL,
        ReceiverUserId INT NOT NULL,
        Status NVARCHAR(30) NOT NULL CONSTRAINT DF_UserInterests_Status DEFAULT ('Pending'),
        CustomMessage NVARCHAR(MAX) NULL,
        SentAt DATETIME2(7) NOT NULL CONSTRAINT DF_UserInterests_SentAt DEFAULT (SYSUTCDATETIME()),
        RespondedAt DATETIME2(7) NULL,
        CONSTRAINT PK_UserInterests PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_UserInterests_Sender FOREIGN KEY (SenderUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserInterests_Receiver FOREIGN KEY (ReceiverUserId) REFERENCES dbo.Users(Id)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 5. TABLE: UserShortlists
-- Description: Bookmarked / Shortlisted profiles
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.UserShortlists', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserShortlists (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        ShortlistedUserId INT NOT NULL,
        ShortlistedAt DATETIME2(7) NOT NULL CONSTRAINT DF_UserShortlists_ShortlistedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserShortlists PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_UserShortlists_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_UserShortlists_ShortlistedUser FOREIGN KEY (ShortlistedUserId) REFERENCES dbo.Users(Id)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 6. TABLE: ChatMessages
-- Description: Real-time SignalR chat messages & read receipts
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.ChatMessages', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChatMessages (
        Id INT IDENTITY(1,1) NOT NULL,
        SenderId INT NOT NULL,
        ReceiverId INT NOT NULL,
        Content NVARCHAR(2000) NOT NULL,
        IsRead BIT NOT NULL CONSTRAINT DF_ChatMessages_IsRead DEFAULT (0),
        SentAt DATETIME2(7) NOT NULL CONSTRAINT DF_ChatMessages_SentAt DEFAULT (SYSUTCDATETIME()),
        ReadAt DATETIME2(7) NULL,
        CONSTRAINT PK_ChatMessages PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ChatMessages_Sender FOREIGN KEY (SenderId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_ChatMessages_Receiver FOREIGN KEY (ReceiverId) REFERENCES dbo.Users(Id)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 7. TABLE: Notifications
-- Description: Notification center alerts (Interests, Messages, Views, Verification)
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        Type NVARCHAR(50) NOT NULL CONSTRAINT DF_Notifications_Type DEFAULT ('Interest'),
        Title NVARCHAR(200) NOT NULL,
        Message NVARCHAR(500) NOT NULL,
        ActionUrl NVARCHAR(250) NULL,
        AvatarUrl NVARCHAR(MAX) NULL,
        IsRead BIT NOT NULL CONSTRAINT DF_Notifications_IsRead DEFAULT (0),
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Notifications_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Notifications PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

-- ----------------------------------------------------------------------------
-- 8. TABLE: ProfileViews
-- Description: Profile visitor history ("Who viewed my profile")
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.ProfileViews', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProfileViews (
        Id INT IDENTITY(1,1) NOT NULL,
        ViewerUserId INT NOT NULL,
        ViewedUserId INT NOT NULL,
        ViewedAt DATETIME2(7) NOT NULL CONSTRAINT DF_ProfileViews_ViewedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ProfileViews PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ProfileViews_Viewer FOREIGN KEY (ViewerUserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_ProfileViews_Viewed FOREIGN KEY (ViewedUserId) REFERENCES dbo.Users(Id)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 9. TABLE: PasswordResetOtps
-- Description: Time-bound 6-digit OTP verification codes
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.PasswordResetOtps', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetOtps (
        Id INT IDENTITY(1,1) NOT NULL,
        Identifier NVARCHAR(150) NOT NULL,
        OtpCode NVARCHAR(6) NOT NULL,
        ExpiresAt DATETIME2(7) NOT NULL,
        IsUsed BIT NOT NULL CONSTRAINT DF_PasswordResetOtps_IsUsed DEFAULT (0),
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_PasswordResetOtps_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PasswordResetOtps PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 10. TABLE: SuccessStories
-- Description: Verified couple success stories featured on homepage & stories page
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.SuccessStories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SuccessStories (
        Id INT IDENTITY(1,1) NOT NULL,
        CoupleName NVARCHAR(150) NOT NULL,
        WeddingDate NVARCHAR(50) NOT NULL,
        Location NVARCHAR(150) NOT NULL,
        ImageUrl NVARCHAR(MAX) NOT NULL,
        Quote NVARCHAR(500) NOT NULL,
        StorySnippet NVARCHAR(MAX) NOT NULL,
        IsFeatured BIT NOT NULL CONSTRAINT DF_SuccessStories_IsFeatured DEFAULT (1),
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_SuccessStories_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_SuccessStories PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 11. MASTER TABLES: MasterReligions, MasterMotherTongues, MasterEducations,
--                    MasterOccupations, MasterIncomeRanges, MasterLocations
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.MasterReligions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterReligions (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        NativeName NVARCHAR(100) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_MasterReligions_SortOrder DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterReligions_Active DEFAULT (1),
        CONSTRAINT PK_MasterReligions PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterMotherTongues', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterMotherTongues (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        NativeName NVARCHAR(100) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_MasterMotherTongues_SortOrder DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterMotherTongues_Active DEFAULT (1),
        CONSTRAINT PK_MasterMotherTongues PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterEducations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterEducations (
        Id INT IDENTITY(1,1) NOT NULL,
        DegreeName NVARCHAR(150) NOT NULL,
        Category NVARCHAR(100) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_MasterEducations_SortOrder DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterEducations_Active DEFAULT (1),
        CONSTRAINT PK_MasterEducations PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterOccupations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterOccupations (
        Id INT IDENTITY(1,1) NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Sector NVARCHAR(100) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_MasterOccupations_SortOrder DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterOccupations_Active DEFAULT (1),
        CONSTRAINT PK_MasterOccupations PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterIncomeRanges', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterIncomeRanges (
        Id INT IDENTITY(1,1) NOT NULL,
        RangeText NVARCHAR(100) NOT NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_MasterIncomeRanges_SortOrder DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterIncomeRanges_Active DEFAULT (1),
        CONSTRAINT PK_MasterIncomeRanges PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterLocations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterLocations (
        Id INT IDENTITY(1,1) NOT NULL,
        CityName NVARCHAR(100) NOT NULL,
        StateName NVARCHAR(100) NOT NULL,
        Country NVARCHAR(100) NOT NULL CONSTRAINT DF_MasterLocations_Country DEFAULT ('India'),
        IsPopular BIT NOT NULL CONSTRAINT DF_MasterLocations_Popular DEFAULT (1),
        SortOrder INT NOT NULL CONSTRAINT DF_MasterLocations_SortOrder DEFAULT (0),
        CONSTRAINT PK_MasterLocations PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- ============================================================================
-- PERFORMANCE & SEARCH OPTIMIZATION INDEXES
-- ============================================================================

-- Users Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON dbo.Users (Email ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Mobile')
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Mobile ON dbo.Users (Mobile ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Gender_Religion_MotherTongue')
    CREATE NONCLUSTERED INDEX IX_Users_Gender_Religion_MotherTongue ON dbo.Users (Gender, Religion, MotherTongue)
    INCLUDE (Name, DateOfBirth, Location, ProfilePhotoUrl, IsVerified);

-- UserProfiles Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserProfiles_UserId')
    CREATE UNIQUE NONCLUSTERED INDEX IX_UserProfiles_UserId ON dbo.UserProfiles (UserId ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserProfiles_Location_Marital_Education')
    CREATE NONCLUSTERED INDEX IX_UserProfiles_Location_Marital_Education ON dbo.UserProfiles (City, State, MaritalStatus, HighestEducation)
    INCLUDE (Occupation, AnnualIncome, Height, Diet);

-- PartnerPreferences Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PartnerPreferences_UserId')
    CREATE UNIQUE NONCLUSTERED INDEX IX_PartnerPreferences_UserId ON dbo.PartnerPreferences (UserId ASC);

-- UserInterests Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserInterests_Sender_Receiver')
    CREATE UNIQUE NONCLUSTERED INDEX IX_UserInterests_Sender_Receiver ON dbo.UserInterests (SenderUserId, ReceiverUserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserInterests_Receiver_Status')
    CREATE NONCLUSTERED INDEX IX_UserInterests_Receiver_Status ON dbo.UserInterests (ReceiverUserId, Status)
    INCLUDE (SenderUserId, SentAt);

-- UserShortlists Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserShortlists_User_Shortlisted')
    CREATE UNIQUE NONCLUSTERED INDEX IX_UserShortlists_User_Shortlisted ON dbo.UserShortlists (UserId, ShortlistedUserId);

-- ChatMessages Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_Conversation')
    CREATE NONCLUSTERED INDEX IX_ChatMessages_Conversation ON dbo.ChatMessages (SenderId, ReceiverId, SentAt ASC)
    INCLUDE (Content, IsRead);

-- Notifications Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId_IsRead')
    CREATE NONCLUSTERED INDEX IX_Notifications_UserId_IsRead ON dbo.Notifications (UserId, IsRead, CreatedAt DESC);

-- ProfileViews Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProfileViews_ViewedUser')
    CREATE NONCLUSTERED INDEX IX_ProfileViews_ViewedUser ON dbo.ProfileViews (ViewedUserId, ViewedAt DESC)
    INCLUDE (ViewerUserId);
GO

PRINT '>>> MilanSetuDB Schema, Tables, Constraints & Indexes Created Successfully! <<<';
GO
