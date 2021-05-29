CREATE TABLE [dbo].[Account] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        INT            NOT NULL,
    [AccountNumber] CHAR(12)    NOT NULL,
    [Balance]       DECIMAL(18, 2)  NOT NULL,
    [CreatedOn]     DATETIME       NOT NULL,
    [UpdatedOn]     DATETIME       NOT NULL,
    [RowVersion]    ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Account_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id])
);

