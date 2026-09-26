-- ============================================================================
-- Project: MilanSetu Matrimony Platform
-- Stored Procedures Suite (Authentication, Profiles, Matches, Search, Interests, Chat)
-- Engine: Microsoft SQL Server 2019 / 2022 / Azure SQL
-- ============================================================================

USE [MilanSetuDB];
GO

-- ----------------------------------------------------------------------------
-- 1. SP: sp_RegisterUser
-- Description: Registers a new user, hashes password and initializes profile defaults
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_RegisterUser
    @Name NVARCHAR(100),
    @Gender NVARCHAR(20),
    @DateOfBirth DATETIME2,
    @Email NVARCHAR(150),
    @Mobile NVARCHAR(20),
    @PasswordHash NVARCHAR(MAX),
    @Religion NVARCHAR(50),
    @Caste NVARCHAR(100) = NULL,
    @MotherTongue NVARCHAR(50),
    @Location NVARCHAR(150),
    @PreferredLanguage NVARCHAR(10) = 'en',
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if email or mobile already exists
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email OR Mobile = @Mobile)
    BEGIN
        RAISERROR('User with this email or mobile already exists.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Users (
            Name, Gender, DateOfBirth, Email, Mobile, PasswordHash, 
            Religion, Caste, MotherTongue, Location, PreferredLanguage, IsVerified, CreatedAt
        )
        VALUES (
            @Name, @Gender, @DateOfBirth, @Email, @Mobile, @PasswordHash, 
            @Religion, @Caste, @MotherTongue, @Location, @PreferredLanguage, 1, SYSUTCDATETIME()
        );

        SET @NewUserId = SCOPE_IDENTITY();

        -- Initialize Default Profile
        INSERT INTO dbo.UserProfiles (UserId, City, State, Country, ProfileCompletionPercentage, UpdatedAt)
        VALUES (@NewUserId, @Location, @Location, 'India', 65, SYSUTCDATETIME());

        -- Initialize Default Partner Preferences
        DECLARE @OppositeGender NVARCHAR(20) = CASE WHEN @Gender = 'Male' THEN 'Female' ELSE 'Male' END;
        INSERT INTO dbo.PartnerPreferences (UserId, Religion, MotherTongue, UpdatedAt)
        VALUES (@NewUserId, @Religion, @MotherTongue, SYSUTCDATETIME());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ----------------------------------------------------------------------------
-- 2. SP: sp_GetUserFullProfile
-- Description: Retrieves consolidated user account and detailed profile details
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_GetUserFullProfile
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id AS UserId,
        u.Name,
        u.Gender,
        u.DateOfBirth,
        DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) AS Age,
        u.Email,
        u.Mobile,
        u.Religion,
        u.Caste,
        u.MotherTongue,
        u.Location,
        u.ProfilePhotoUrl,
        u.IsVerified,
        u.PreferredLanguage,
        u.CreatedAt,
        -- Profile Info
        p.Id AS ProfileId,
        p.Height,
        p.Weight,
        p.MaritalStatus,
        p.PhysicalStatus,
        p.ProfileManagedBy,
        p.AboutMe,
        p.PartnerExpectations,
        p.HighestEducation,
        p.CollegeOrUniversity,
        p.FieldOfStudy,
        p.EmployedIn,
        p.Occupation,
        p.CompanyName,
        p.WorkLocation,
        p.AnnualIncome,
        p.FamilyType,
        p.FamilyValues,
        p.FatherOccupation,
        p.MotherOccupation,
        p.NumberOfBrothers,
        p.NumberOfSisters,
        p.FamilyCity,
        p.Diet,
        p.Drink,
        p.Smoke,
        p.Hobbies,
        p.SubCasteOrGothra,
        p.ManglikStatus,
        p.Rashi,
        p.Nakshatra,
        p.City,
        p.State,
        p.Country,
        p.NativePlace,
        p.WillingToRelocate,
        p.PhotoGalleryJson,
        p.ProfileCompletionPercentage
    FROM dbo.Users u
    LEFT JOIN dbo.UserProfiles p ON u.Id = p.UserId
    WHERE u.Id = @UserId;
END;
GO

-- ----------------------------------------------------------------------------
-- 3. SP: sp_SearchMatrimonyProfiles
-- Description: Advanced search across all matrimonial criteria with pagination
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_SearchMatrimonyProfiles
    @CurrentUserId INT = NULL,
    @Gender NVARCHAR(20) = NULL,
    @MinAge INT = 18,
    @MaxAge INT = 70,
    @Religion NVARCHAR(50) = NULL,
    @MotherTongue NVARCHAR(50) = NULL,
    @MaritalStatus NVARCHAR(50) = NULL,
    @City NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @Education NVARCHAR(100) = NULL,
    @Occupation NVARCHAR(100) = NULL,
    @ProfileIdSearch INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id,
        u.Name,
        u.Gender,
        u.DateOfBirth,
        DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) AS Age,
        u.Religion,
        u.Caste,
        u.MotherTongue,
        u.Location,
        u.ProfilePhotoUrl,
        u.IsVerified,
        p.Height,
        p.MaritalStatus,
        p.HighestEducation,
        p.Occupation,
        p.AnnualIncome,
        p.City,
        p.State,
        p.AboutMe,
        p.Diet,
        -- Shortlist status for current user
        CASE WHEN s.Id IS NOT NULL THEN 1 ELSE 0 END AS IsShortlisted,
        -- Interest status
        COALESCE(i.Status, 'None') AS InterestStatus
    FROM dbo.Users u
    LEFT JOIN dbo.UserProfiles p ON u.Id = p.UserId
    LEFT JOIN dbo.UserShortlists s ON s.UserId = @CurrentUserId AND s.ShortlistedUserId = u.Id
    LEFT JOIN dbo.UserInterests i ON (i.SenderUserId = @CurrentUserId AND i.ReceiverUserId = u.Id) 
                                  OR (i.SenderUserId = u.Id AND i.ReceiverUserId = @CurrentUserId)
    WHERE (@CurrentUserId IS NULL OR u.Id <> @CurrentUserId)
      AND (@ProfileIdSearch IS NULL OR u.Id = @ProfileIdSearch)
      AND (@Gender IS NULL OR @Gender = 'All' OR u.Gender = @Gender)
      AND (DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) BETWEEN @MinAge AND @MaxAge)
      AND (@Religion IS NULL OR @Religion = 'All' OR @Religion = 'Any Religion' OR u.Religion = @Religion)
      AND (@MotherTongue IS NULL OR @MotherTongue = 'All' OR @MotherTongue = 'Any Language' OR u.MotherTongue = @MotherTongue)
      AND (@MaritalStatus IS NULL OR @MaritalStatus = 'All' OR p.MaritalStatus = @MaritalStatus)
      AND (@City IS NULL OR p.City LIKE '%' + @City + '%' OR u.Location LIKE '%' + @City + '%')
      AND (@Education IS NULL OR p.HighestEducation LIKE '%' + @Education + '%')
      AND (@Occupation IS NULL OR p.Occupation LIKE '%' + @Occupation + '%')
    ORDER BY u.CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- ----------------------------------------------------------------------------
-- 4. SP: sp_SendInterest
-- Description: Sends an express interest, checks duplicates, inserts notification
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_SendInterest
    @SenderUserId INT,
    @ReceiverUserId INT,
    @CustomMessage NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SenderUserId = @ReceiverUserId
    BEGIN
        RAISERROR('You cannot send interest to yourself.', 16, 1);
        RETURN;
    END

    -- Check if interest already exists
    IF EXISTS (SELECT 1 FROM dbo.UserInterests WHERE SenderUserId = @SenderUserId AND ReceiverUserId = @ReceiverUserId)
    BEGIN
        -- Update existing to pending if withdrawn
        UPDATE dbo.UserInterests
        SET Status = 'Pending', CustomMessage = @CustomMessage, SentAt = SYSUTCDATETIME()
        WHERE SenderUserId = @SenderUserId AND ReceiverUserId = @ReceiverUserId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.UserInterests (SenderUserId, ReceiverUserId, Status, CustomMessage, SentAt)
        VALUES (@SenderUserId, @ReceiverUserId, 'Pending', @CustomMessage, SYSUTCDATETIME());
    END

    -- Create Notification for Receiver
    DECLARE @SenderName NVARCHAR(100);
    DECLARE @SenderAvatar NVARCHAR(MAX);
    SELECT @SenderName = Name, @SenderAvatar = ProfilePhotoUrl FROM dbo.Users WHERE Id = @SenderUserId;

    INSERT INTO dbo.Notifications (UserId, Type, Title, Message, ActionUrl, AvatarUrl, IsRead, CreatedAt)
    VALUES (
        @ReceiverUserId, 
        'Interest', 
        'New Interest Received! ❤️', 
        @SenderName + ' expressed interest in your profile.', 
        '/interests', 
        @SenderAvatar, 
        0, 
        SYSUTCDATETIME()
    );
END;
GO

-- ----------------------------------------------------------------------------
-- 5. SP: sp_RespondInterest
-- Description: Accepts or Declines an interest request and notifies the sender
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_RespondInterest
    @ReceiverUserId INT,
    @SenderUserId INT,
    @ResponseStatus NVARCHAR(30) -- 'Accepted' or 'Declined'
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.UserInterests
    SET Status = @ResponseStatus, RespondedAt = SYSUTCDATETIME()
    WHERE SenderUserId = @SenderUserId AND ReceiverUserId = @ReceiverUserId;

    -- If accepted, send notification to sender
    IF @ResponseStatus = 'Accepted'
    BEGIN
        DECLARE @ReceiverName NVARCHAR(100);
        DECLARE @ReceiverAvatar NVARCHAR(MAX);
        SELECT @ReceiverName = Name, @ReceiverAvatar = ProfilePhotoUrl FROM dbo.Users WHERE Id = @ReceiverUserId;

        INSERT INTO dbo.Notifications (UserId, Type, Title, Message, ActionUrl, AvatarUrl, IsRead, CreatedAt)
        VALUES (
            @SenderUserId, 
            'Interest', 
            'Interest Accepted! 🎉', 
            @ReceiverName + ' accepted your interest! You can now start chatting and view contact details.', 
            '/chat', 
            @ReceiverAvatar, 
            0, 
            SYSUTCDATETIME()
        );
    END
END;
GO

-- ----------------------------------------------------------------------------
-- 6. SP: sp_ToggleShortlist
-- Description: Adds or removes a profile from user's shortlist
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_ToggleShortlist
    @UserId INT,
    @ShortlistedUserId INT,
    @IsShortlisted BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.UserShortlists WHERE UserId = @UserId AND ShortlistedUserId = @ShortlistedUserId)
    BEGIN
        DELETE FROM dbo.UserShortlists WHERE UserId = @UserId AND ShortlistedUserId = @ShortlistedUserId;
        SET @IsShortlisted = 0;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.UserShortlists (UserId, ShortlistedUserId, ShortlistedAt)
        VALUES (@UserId, @ShortlistedUserId, SYSUTCDATETIME());
        SET @IsShortlisted = 1;
    END
END;
GO

-- ----------------------------------------------------------------------------
-- 7. SP: sp_GetChatHistory
-- Description: Retrieves conversation between two users and marks messages as read
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_GetChatHistory
    @UserId INT,
    @PartnerId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Mark incoming messages as read
    UPDATE dbo.ChatMessages
    SET IsRead = 1, ReadAt = SYSUTCDATETIME()
    WHERE SenderId = @PartnerId AND ReceiverId = @UserId AND IsRead = 0;

    -- Return full message log
    SELECT 
        Id,
        SenderId,
        ReceiverId,
        Content,
        IsRead,
        SentAt,
        ReadAt
    FROM dbo.ChatMessages
    WHERE (SenderId = @UserId AND ReceiverId = @PartnerId)
       OR (SenderId = @PartnerId AND ReceiverId = @UserId)
    ORDER BY SentAt ASC;
END;
GO

-- ----------------------------------------------------------------------------
-- 8. SP: sp_SendMessage
-- Description: Inserts a chat message, returns inserted record ID
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_SendMessage
    @SenderId INT,
    @ReceiverId INT,
    @Content NVARCHAR(2000),
    @NewMessageId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ChatMessages (SenderId, ReceiverId, Content, IsRead, SentAt)
    VALUES (@SenderId, @ReceiverId, @Content, 0, SYSUTCDATETIME());

    SET @NewMessageId = SCOPE_IDENTITY();
END;
GO

-- ----------------------------------------------------------------------------
-- 9. SP: sp_GetSuccessStories
-- Description: Fetches all featured couple success stories
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_GetSuccessStories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        CoupleName,
        WeddingDate,
        Location,
        ImageUrl,
        Quote,
        StorySnippet,
        IsFeatured,
        CreatedAt
    FROM dbo.SuccessStories
    WHERE IsFeatured = 1
    ORDER BY CreatedAt DESC;
END;
GO

-- ----------------------------------------------------------------------------
-- 10. SP: sp_RecordProfileView
-- Description: Records a profile view event and creates notification for the viewed member
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_RecordProfileView
    @ViewerUserId INT,
    @ViewedUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @ViewerUserId = @ViewedUserId RETURN;

    -- Insert Profile View log
    INSERT INTO dbo.ProfileViews (ViewerUserId, ViewedUserId, ViewedAt)
    VALUES (@ViewerUserId, @ViewedUserId, SYSUTCDATETIME());

    -- Create Notification with Matrimony ID
    DECLARE @ViewerName NVARCHAR(100);
    DECLARE @ViewerAvatar NVARCHAR(MAX);
    SELECT @ViewerName = Name, @ViewerAvatar = ProfilePhotoUrl FROM dbo.Users WHERE Id = @ViewerUserId;

    INSERT INTO dbo.Notifications (UserId, Type, Title, Message, ActionUrl, AvatarUrl, IsRead, CreatedAt)
    VALUES (
        @ViewedUserId,
        'ProfileView',
        '👀 Profile Viewed!',
        'Member MS-' + CAST(@ViewerUserId AS NVARCHAR(20)) + ' (' + @ViewerName + ') just viewed your profile.',
        '/search?id=' + CAST(@ViewerUserId AS NVARCHAR(20)),
        @ViewerAvatar,
        0,
        SYSUTCDATETIME()
    );
END;
GO

-- ----------------------------------------------------------------------------
-- 11. SP: sp_GetNotifications
-- Description: Retrieves user notifications ordered by newest first
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_GetNotifications
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Type,
        Title,
        Message,
        ActionUrl,
        AvatarUrl,
        IsRead,
        CreatedAt
    FROM dbo.Notifications
    WHERE UserId = @UserId
    ORDER BY CreatedAt DESC;
END;
GO

-- ----------------------------------------------------------------------------
-- 12. SP: sp_SavePasswordResetOtp
-- Description: Stores 6-digit OTP code for password recovery
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_SavePasswordResetOtp
    @Identifier NVARCHAR(150),
    @OtpCode NVARCHAR(6),
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Invalidate existing OTPs for identifier
    UPDATE dbo.PasswordResetOtps
    SET IsUsed = 1
    WHERE Identifier = @Identifier AND IsUsed = 0;

    INSERT INTO dbo.PasswordResetOtps (Identifier, OtpCode, ExpiresAt, IsUsed, CreatedAt)
    VALUES (@Identifier, @OtpCode, @ExpiresAt, 0, SYSUTCDATETIME());
END;
GO

-- ----------------------------------------------------------------------------
-- 13. SP: sp_VerifyPasswordResetOtp
-- Description: Validates entered OTP code
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.sp_VerifyPasswordResetOtp
    @Identifier NVARCHAR(150),
    @OtpCode NVARCHAR(6),
    @IsValid BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.PasswordResetOtps
        WHERE Identifier = @Identifier 
          AND OtpCode = @OtpCode 
          AND IsUsed = 0 
          AND ExpiresAt > SYSUTCDATETIME()
    )
    BEGIN
        SET @IsValid = 1;
        UPDATE dbo.PasswordResetOtps
        SET IsUsed = 1
        WHERE Identifier = @Identifier AND OtpCode = @OtpCode;
    END
    ELSE
    BEGIN
        SET @IsValid = 0;
    END
END;
GO

PRINT '>>> All MilanSetu Stored Procedures Compiled Successfully! <<<';
GO
