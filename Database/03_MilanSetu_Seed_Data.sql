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

-- Insert Success Stories
IF NOT EXISTS (SELECT 1 FROM dbo.SuccessStories)
BEGIN
    INSERT INTO dbo.SuccessStories (CoupleName, WeddingDate, Location, ImageUrl, Quote, StorySnippet, IsFeatured, CreatedAt)
    VALUES
    (N'Vikram & Radhika', N'December 2025', N'Jaipur Palace, Rajasthan', N'https://images.unsplash.com/photo-1583939003579-730e3918a45a?auto=format&fit=crop&w=600&q=80', N'"We connected on MilanSetu with just one click, and found a lifetime of unconditional love and laughter!"', N'Vikram from Pune and Radhika from Jaipur matched through verified filters. Their shared love for travel and family values led to a beautiful destination wedding.', 1, SYSUTCDATETIME()),
    (N'Aman & Harpreet', N'November 2025', N'Amritsar, Punjab', N'https://images.unsplash.com/photo-1609357605129-26f69add5d6e?auto=format&fit=crop&w=600&q=80', N'"MilanSetu’s verified profiles gave our families 100% peace of mind and the perfect life companion."', N'Both working in healthcare, they found true alignment in aspirations and core Punjabi values within 3 weeks of connecting on the portal.', 1, SYSUTCDATETIME()),
    (N'Arjun & Sneha', N'January 2026', N'Udaipur, Rajasthan', N'https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=600&q=80', N'"Found my soulmate who understands my career goals and cherishes cultural traditions equally."', N'From our first chat on MilanSetu to meeting each other’s families, everything felt naturally right. Forever grateful!', 1, SYSUTCDATETIME());

    PRINT '>>> Success Stories Seeded Successfully! <<<';
END
GO

-- Insert Master Religions
IF NOT EXISTS (SELECT 1 FROM dbo.MasterReligions)
BEGIN
    INSERT INTO dbo.MasterReligions (Name, DisplayOrder, IsActive)
    VALUES 
    (N'Hindu', 1, 1), (N'Muslim', 2, 1), (N'Sikh', 3, 1), (N'Christian', 4, 1), 
    (N'Jain', 5, 1), (N'Buddhist', 6, 1), (N'Parsi', 7, 1), (N'Jewish', 8, 1), (N'Other', 9, 1);
END
GO

-- Insert Master Mother Tongues
IF NOT EXISTS (SELECT 1 FROM dbo.MasterMotherTongues)
BEGIN
    INSERT INTO dbo.MasterMotherTongues (Name, DisplayOrder, IsActive)
    VALUES 
    (N'Hindi', 1, 1), (N'Bengali', 2, 1), (N'Marathi', 3, 1), (N'Telugu', 4, 1), 
    (N'Tamil', 5, 1), (N'Gujarati', 6, 1), (N'Urdu', 7, 1), (N'Kannada', 8, 1), 
    (N'Odia', 9, 1), (N'Malayalam', 10, 1), (N'Punjabi', 11, 1), (N'Assamese', 12, 1), 
    (N'Maithili', 13, 1), (N'English', 14, 1);
END
GO

-- Insert Master Educations
IF NOT EXISTS (SELECT 1 FROM dbo.MasterEducations)
BEGIN
    INSERT INTO dbo.MasterEducations (Name, DisplayOrder, IsActive)
    VALUES 
    (N'B.Tech / B.E / B.S', 1, 1), (N'M.Tech / M.E / M.S', 2, 1), (N'MBA / PGDM', 3, 1), 
    (N'BCA / MCA / B.Sc IT', 4, 1), (N'MBBS / MD / MS / BDS', 5, 1), (N'CA / CS / ICWA / CFA', 6, 1), 
    (N'B.Com / M.Com', 7, 1), (N'B.A / M.A', 8, 1), (N'LLB / LLM', 9, 1), 
    (N'Ph.D / Doctorate', 10, 1), (N'Diploma / Polytechnic', 11, 1), (N'Higher Secondary / 12th', 12, 1);
END
GO

-- Insert Master Occupations
IF NOT EXISTS (SELECT 1 FROM dbo.MasterOccupations)
BEGIN
    INSERT INTO dbo.MasterOccupations (Name, DisplayOrder, IsActive)
    VALUES 
    (N'Software Engineer / Architect', 1, 1), (N'Data Scientist / AI Specialist', 2, 1), 
    (N'Doctor / Surgeon / Healthcare', 3, 1), (N'Chartered Accountant / Finance', 4, 1), 
    (N'Civil Services / IAS / IPS / Govt', 5, 1), (N'Business Owner / Entrepreneur', 6, 1), 
    (N'Professor / Lecturer / Teacher', 7, 1), (N'Marketing & Product Manager', 8, 1), 
    (N'Banker / Financial Analyst', 9, 1), (N'Lawyer / Legal Advisor', 10, 1), 
    (N'Architect / Interior Designer', 11, 1), (N'Defense Forces (Army/Navy/Air Force)', 12, 1);
END
GO

-- Insert Master Income Ranges
IF NOT EXISTS (SELECT 1 FROM dbo.MasterIncomeRanges)
BEGIN
    INSERT INTO dbo.MasterIncomeRanges (RangeText, DisplayOrder, IsActive)
    VALUES 
    (N'₹3 - ₹5 Lakhs', 1, 1), (N'₹5 - ₹7 Lakhs', 2, 1), (N'₹7 - ₹10 Lakhs', 3, 1), 
    (N'₹10 - ₹15 Lakhs', 4, 1), (N'₹15 - ₹25 Lakhs', 5, 1), (N'₹25 - ₹50 Lakhs', 6, 1), 
    (N'₹50 Lakhs - ₹1 Crore', 7, 1), (N'₹1 Crore & Above', 8, 1);
END
GO

-- Insert Master Locations
IF NOT EXISTS (SELECT 1 FROM dbo.MasterLocations)
BEGIN
    INSERT INTO dbo.MasterLocations (CityName, StateName, DisplayOrder, IsActive)
    VALUES 
    (N'Mumbai', N'Maharashtra', 1, 1), (N'Delhi NCR', N'Delhi', 2, 1), 
    (N'Bengaluru', N'Karnataka', 3, 1), (N'Pune', N'Maharashtra', 4, 1), 
    (N'Hyderabad', N'Telangana', 5, 1), (N'Chennai', N'Tamil Nadu', 6, 1), 
    (N'Kolkata', N'West Bengal', 7, 1), (N'Ahmedabad', N'Gujarat', 8, 1), 
    (N'Jaipur', N'Rajasthan', 9, 1), (N'Lucknow', N'Uttar Pradesh', 10, 1), 
    (N'Chandigarh', N'Punjab', 11, 1), (N'Indore', N'Madhya Pradesh', 12, 1);
END
GO

PRINT '>>> All MilanSetu Seed & Master Data Inserted Successfully! <<<';
GO

