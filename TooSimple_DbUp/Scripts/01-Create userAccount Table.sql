CREATE TABLE UserAccounts (
UserAccountId CHAR(40) NOT NULL PRIMARY KEY,
UserName NVARCHAR(255) NOT NULL,
NormalizedUserName NVARCHAR(255) NOT NULL,
Email NVARCHAR(255) NOT NULL,
NormalizedEmail NVARCHAR(255) NOT NULL,
IsEmailConfirmed BOOLEAN,
PasswordHash CHAR(84),
PhoneNumber NVARCHAR(255),
IsPhoneConfirmed NVARCHAR(255),
IsTwoFactorEnabled NVARCHAR(255),
FailedLoginCount INT
)
