-- ============================================================================
-- Project: MilanSetu Matrimony Platform
-- Seed Data Script (Demo Profiles across Religions, Professions, Locations)
-- ============================================================================

USE [MilanSetuDB];
GO

-- Insert Demo Users if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'priya.sharma@example.com')
BEGIN
    INSERT INTO dbo.Users (Name, Gender, DateOfBirth, Email, Mobile, PasswordHash, Religion, Caste, MotherTongue, Location, ProfilePhotoUrl, IsVerified, PreferredLanguage, CreatedAt)
    VALUES 
    (N'Priya Sharma', N'Female', '1998-05-14', N'priya.sharma@example.com', N'9876543210', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Brahmin', N'Hindi', N'Delhi, India', N'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=500&auto=format&fit=crop&q=80', 1, 'hi', SYSUTCDATETIME()),
    (N'Ananya Deshmukh', N'Female', '1997-11-20', N'ananya.d@example.com', N'9876543211', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Maratha', N'Marathi', N'Pune, Maharashtra', N'https://images.unsplash.com/photo-1517841905240-472988babdf9?w=500&auto=format&fit=crop&q=80', 1, 'mr', SYSUTCDATETIME()),
    (N'Sneha Reddy', N'Female', '1999-02-18', N'sneha.reddy@example.com', N'9876543212', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Reddy', N'Telugu', N'Hyderabad, Telangana', N'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=500&auto=format&fit=crop&q=80', 1, 'te', SYSUTCDATETIME()),
    (N'Aarav Patel', N'Male', '1995-08-22', N'aarav.patel@example.com', N'9876543213', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Patel / Leva', N'Gujarati', N'Ahmedabad, Gujarat', N'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=500&auto=format&fit=crop&q=80', 1, 'gu', SYSUTCDATETIME()),
    (N'Karthik Sundaram', N'Male', '1994-04-10', N'karthik.s@example.com', N'9876543214', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Iyer', N'Tamil', N'Chennai, Tamil Nadu', N'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=500&auto=format&fit=crop&q=80', 1, 'ta', SYSUTCDATETIME()),
    (N'Debjani Banerjee', N'Female', '1996-09-05', N'debjani.b@example.com', N'9876543215', N'$2a$11$e87vBq3u0uB9R.E4Qd0GquN4rO3t7N9fD.qK5u7O6a8Q9m3Z7L2Wy', N'Hindu', N'Bengali Brahmin', N'Bengali', N'Kolkata, West Bengal', N'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?w=500&auto=format&fit=crop&q=80', 1, 'bn', SYSUTCDATETIME());

    PRINT '>>> Seed Users Inserted Successfully! <<<';
END
GO
