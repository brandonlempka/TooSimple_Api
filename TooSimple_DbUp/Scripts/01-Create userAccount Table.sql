CREATE TABLE UserAccounts (
UserAccountId CHAR(40) NOT NULL PRIMARY KEY,
UserName NVARCHAR(255) NOT NULL,
NormalizedUserName NVARCHAR(255) NOT NULL UNIQUE,
Email NVARCHAR(255) NOT NULL,
NormalizedEmail NVARCHAR(255) NOT NULL UNIQUE,
IsEmailConfirmed BOOLEAN,
PasswordHash CHAR(88),
PasswordSalt CHAR(88),
PhoneNumber NVARCHAR(255),
IsPhoneConfirmed BOOLEAN(255),
IsTwoFactorEnabled BOOLEAN(255),
FailedLoginCount INT
)
