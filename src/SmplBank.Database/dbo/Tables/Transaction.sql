CREATE TABLE [dbo].[Transaction] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]             INT             NOT NULL,
    [Type]                  INT             NOT NULL,
    [Status]                INT             NOT NULL,
    [Description]           NVARCHAR(2000)  NULL,
    [Amount]                DECIMAL(18, 2)  NOT NULL,
    [EndBalance]            DECIMAL(18, 2)  NULL,
    [LinkedTransactionId]   INT             NULL,
    [CreatedOn]             DATETIME        NOT NULL,
    [UpdatedOn]             DATETIME        NOT NULL,
    [RowVersion]            ROWVERSION      NOT NULL,
    CONSTRAINT [PK_Transaction_Id]          PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Transaction_Account]     FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account]([Id]),
    CONSTRAINT [FK_Transaction_Transaction] FOREIGN KEY ([LinkedTransactionId]) REFERENCES [dbo].[Transaction]([Id])
);

