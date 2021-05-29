CREATE TABLE [dbo].[User] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Username]   VARCHAR (50)   NOT NULL,
    [Password]   VARCHAR (500)  NOT NULL,
    [CreatedOn]  DATETIME       NOT NULL,
    [UpdatedOn]  DATETIME       NOT NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_User_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_User_Username] UNIQUE NONCLUSTERED ([Username] ASC)
);

