CREATE TABLE [dbo].[Customer] (
    [CustomerGuid] UNIQUEIDENTIFIER NOT NULL,
    [Name]         VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerGuid] ASC)
);

