-- ============================================================================
-- MILANSETU MATRIMONY - MASTER DATABASE INITIALIZATION SCRIPT
-- Contains: Database Creation, Tables, Indexes, Constraints, and Stored Procedures
-- Compatible: SQL Server 2019+, Azure SQL, SQL Express
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MilanSetuDB')
BEGIN
    CREATE DATABASE [MilanSetuDB];
END
GO

USE [MilanSetuDB];
GO

-- 1. USERS TABLE
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
        PreferredLanguage NVARCHAR(10) NOT NULL CONSTRAINT DF_Users_PreferredLanguage DEFAULT ('en'),
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- 2. USER PROFILES TABLE
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfiles (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        Height NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Height DEFAULT ('5''7"'),
        Weight NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Weight DEFAULT ('65 kg'),
        MaritalStatus NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_MaritalStatus DEFAULT ('Never Married'),
        PhysicalStatus NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_PhysicalStatus DEFAULT ('Normal'),
        ProfileManagedBy NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_ProfileManagedBy DEFAULT ('Self'),
        AboutMe NVARCHAR(1000) NULL,
        PartnerExpectations NVARCHAR(500) NULL,
        HighestEducation NVARCHAR(100) NULL,
        CollegeOrUniversity NVARCHAR(150) NULL,
        FieldOfStudy NVARCHAR(100) NULL,
        EmployedIn NVARCHAR(50) NULL CONSTRAINT DF_UserProfiles_EmployedIn DEFAULT ('Private Sector'),
        Occupation NVARCHAR(100) NULL,
        CompanyName NVARCHAR(150) NULL,
        WorkLocation NVARCHAR(100) NULL,
        AnnualIncome NVARCHAR(50) NULL CONSTRAINT DF_UserProfiles_AnnualIncome DEFAULT (N'₹10 - ₹15 Lakhs'),
        FamilyType NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_FamilyType DEFAULT ('Nuclear'),
        FamilyValues NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_FamilyValues DEFAULT ('Moderate'),
        FatherOccupation NVARCHAR(100) NULL,
        MotherOccupation NVARCHAR(100) NULL,
        NumberOfBrothers INT NOT NULL CONSTRAINT DF_UserProfiles_Brothers DEFAULT (0),
        NumberOfSisters INT NOT NULL CONSTRAINT DF_UserProfiles_Sisters DEFAULT (0),
        FamilyCity NVARCHAR(150) NULL,
        Diet NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Diet DEFAULT ('Vegetarian'),
        Drink NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Drink DEFAULT ('No'),
        Smoke NVARCHAR(30) NULL CONSTRAINT DF_UserProfiles_Smoke DEFAULT ('No'),
        Hobbies NVARCHAR(300) NULL CONSTRAINT DF_UserProfiles_Hobbies DEFAULT ('Travelling, Reading, Music, Fitness'),
        SubCasteOrGothra NVARCHAR(100) NULL,
        ManglikStatus NVARCHAR(20) NULL CONSTRAINT DF_UserProfiles_Manglik DEFAULT ('No'),
        Rashi NVARCHAR(50) NULL,
        Nakshatra NVARCHAR(50) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(100) NULL,
        Country NVARCHAR(100) NULL CONSTRAINT DF_UserProfiles_Country DEFAULT ('India'),
        NativePlace NVARCHAR(100) NULL,
        WillingToRelocate BIT NOT NULL CONSTRAINT DF_UserProfiles_Relocate DEFAULT (1),
        PhotoGalleryJson NVARCHAR(MAX) NULL,
        ProfileCompletionPercentage INT NOT NULL CONSTRAINT DF_UserProfiles_Completion DEFAULT (85),
        UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_UserProfiles_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserProfiles PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_UserProfiles_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

-- 3. PARTNER PREFERENCES TABLE
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

-- 4. USER INTERESTS TABLE
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

-- 5. USER SHORTLISTS TABLE
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

-- 6. CHAT MESSAGES TABLE
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

-- 7. NOTIFICATIONS TABLE
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

-- 8. PROFILE VIEWS TABLE
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

-- 9. PASSWORD RESET OTPS TABLE
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

-- 10. SUCCESS STORIES TABLE
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

-- 11. MASTER DROPDOWN TABLES
IF OBJECT_ID('dbo.MasterReligions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterReligions (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterReligions_Order DEFAULT (0),
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
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterMotherTongues_Order DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterMotherTongues_Active DEFAULT (1),
        CONSTRAINT PK_MasterMotherTongues PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterEducations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterEducations (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterEducations_Order DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterEducations_Active DEFAULT (1),
        CONSTRAINT PK_MasterEducations PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

IF OBJECT_ID('dbo.MasterOccupations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MasterOccupations (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterOccupations_Order DEFAULT (0),
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
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterIncomeRanges_Order DEFAULT (0),
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
        StateName NVARCHAR(100) NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_MasterLocations_Order DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_MasterLocations_Active DEFAULT (1),
        CONSTRAINT PK_MasterLocations PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- INDEXES
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON dbo.Users (Email ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Mobile')
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Mobile ON dbo.Users (Mobile ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Gender_Religion_MotherTongue')
    CREATE NONCLUSTERED INDEX IX_Users_Gender_Religion_MotherTongue ON dbo.Users (Gender, Religion, MotherTongue)
    INCLUDE (Name, DateOfBirth, Location, ProfilePhotoUrl, IsVerified);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserProfiles_UserId')
    CREATE UNIQUE NONCLUSTERED INDEX IX_UserProfiles_UserId ON dbo.UserProfiles (UserId ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PartnerPreferences_UserId')
    CREATE UNIQUE NONCLUSTERED INDEX IX_PartnerPreferences_UserId ON dbo.PartnerPreferences (UserId ASC);

-- SEED SUCCESS STORIES
IF NOT EXISTS (SELECT 1 FROM dbo.SuccessStories)
BEGIN
    INSERT INTO dbo.SuccessStories (CoupleName, WeddingDate, Location, ImageUrl, Quote, StorySnippet, IsFeatured, CreatedAt)
    VALUES
    (N'Vikram & Radhika', N'December 2025', N'Jaipur Palace, Rajasthan', N'https://images.unsplash.com/photo-1583939003579-730e3918a45a?auto=format&fit=crop&w=600&q=80', N'"We connected on MilanSetu with just one click, and found a lifetime of unconditional love and laughter!"', N'Vikram from Pune and Radhika from Jaipur matched through verified filters. Their shared love for travel and family values led to a beautiful destination wedding.', 1, SYSUTCDATETIME()),
    (N'Aman & Harpreet', N'November 2025', N'Amritsar, Punjab', N'https://images.unsplash.com/photo-1609357605129-26f69add5d6e?auto=format&fit=crop&w=600&q=80', N'"MilanSetu’s verified profiles gave our families 100% peace of mind and the perfect life companion."', N'Both working in healthcare, they found true alignment in aspirations and core Punjabi values within 3 weeks of connecting on the portal.', 1, SYSUTCDATETIME()),
    (N'Arjun & Sneha', N'January 2026', N'Udaipur, Rajasthan', N'https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=600&q=80', N'"Found my soulmate who understands my career goals and cherishes cultural traditions equally."', N'From our first chat on MilanSetu to meeting each other’s families, everything felt naturally right. Forever grateful!', 1, SYSUTCDATETIME());
END

-- SEED MASTER DATA
IF NOT EXISTS (SELECT 1 FROM dbo.MasterReligions)
BEGIN
    INSERT INTO dbo.MasterReligions (Name, DisplayOrder, IsActive) VALUES 
    (N'Hindu', 1, 1), (N'Muslim', 2, 1), (N'Sikh', 3, 1), (N'Christian', 4, 1), 
    (N'Jain', 5, 1), (N'Buddhist', 6, 1), (N'Parsi', 7, 1), (N'Jewish', 8, 1), (N'Other', 9, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MasterMotherTongues)
BEGIN
    INSERT INTO dbo.MasterMotherTongues (Name, DisplayOrder, IsActive) VALUES 
    (N'Hindi', 1, 1), (N'Bengali', 2, 1), (N'Marathi', 3, 1), (N'Telugu', 4, 1), 
    (N'Tamil', 5, 1), (N'Gujarati', 6, 1), (N'Urdu', 7, 1), (N'Kannada', 8, 1), 
    (N'Odia', 9, 1), (N'Malayalam', 10, 1), (N'Punjabi', 11, 1), (N'Assamese', 12, 1), 
    (N'Maithili', 13, 1), (N'English', 14, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MasterEducations)
BEGIN
    INSERT INTO dbo.MasterEducations (Name, DisplayOrder, IsActive) VALUES 
    (N'B.Tech / B.E / B.S', 1, 1), (N'M.Tech / M.E / M.S', 2, 1), (N'MBA / PGDM', 3, 1), 
    (N'BCA / MCA / B.Sc IT', 4, 1), (N'MBBS / MD / MS / BDS', 5, 1), (N'CA / CS / ICWA / CFA', 6, 1), 
    (N'B.Com / M.Com', 7, 1), (N'B.A / M.A', 8, 1), (N'LLB / LLM', 9, 1), 
    (N'Ph.D / Doctorate', 10, 1), (N'Diploma / Polytechnic', 11, 1), (N'Higher Secondary / 12th', 12, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MasterOccupations)
BEGIN
    INSERT INTO dbo.MasterOccupations (Name, DisplayOrder, IsActive) VALUES 
    (N'Software Engineer / Architect', 1, 1), (N'Data Scientist / AI Specialist', 2, 1), 
    (N'Doctor / Surgeon / Healthcare', 3, 1), (N'Chartered Accountant / Finance', 4, 1), 
    (N'Civil Services / IAS / IPS / Govt', 5, 1), (N'Business Owner / Entrepreneur', 6, 1), 
    (N'Professor / Lecturer / Teacher', 7, 1), (N'Marketing & Product Manager', 8, 1), 
    (N'Banker / Financial Analyst', 9, 1), (N'Lawyer / Legal Advisor', 10, 1), 
    (N'Architect / Interior Designer', 11, 1), (N'Defense Forces (Army/Navy/Air Force)', 12, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MasterIncomeRanges)
BEGIN
    INSERT INTO dbo.MasterIncomeRanges (RangeText, DisplayOrder, IsActive) VALUES 
    (N'₹3 - ₹5 Lakhs', 1, 1), (N'₹5 - ₹7 Lakhs', 2, 1), (N'₹7 - ₹10 Lakhs', 3, 1), 
    (N'₹10 - ₹15 Lakhs', 4, 1), (N'₹15 - ₹25 Lakhs', 5, 1), (N'₹25 - ₹50 Lakhs', 6, 1), 
    (N'₹50 Lakhs - ₹1 Crore', 7, 1), (N'₹1 Crore & Above', 8, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MasterLocations)
BEGIN
    INSERT INTO dbo.MasterLocations (CityName, StateName, DisplayOrder, IsActive) VALUES 
    (N'Mumbai', N'Maharashtra', 1, 1), (N'Delhi NCR', N'Delhi', 2, 1), 
    (N'Bengaluru', N'Karnataka', 3, 1), (N'Pune', N'Maharashtra', 4, 1), 
    (N'Hyderabad', N'Telangana', 5, 1), (N'Chennai', N'Tamil Nadu', 6, 1), 
    (N'Kolkata', N'West Bengal', 7, 1), (N'Ahmedabad', N'Gujarat', 8, 1), 
    (N'Jaipur', N'Rajasthan', 9, 1), (N'Lucknow', N'Uttar Pradesh', 10, 1), 
    (N'Chandigarh', N'Punjab', 11, 1), (N'Indore', N'Madhya Pradesh', 12, 1);
END

PRINT '>>> All MilanSetu Tables, Master Data & Seed Data Initialized! <<<';
GO
