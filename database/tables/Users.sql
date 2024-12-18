IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO

CREATE TABLE [dbo].[Users](
    [UserId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [EmployeeNum] [varchar](12) NOT NULL,
    [FirstName] [varchar](50) NULL,
    [LastName] [varchar](50) NULL,
    [Email] [varchar](50) NULL,
    [UtcCreatedAt] [datetime] NULL,
    [CreatedBy] [varchar](50) NULL,
    [UtcUpdatedAt] [datetime] NULL,
    [UpdatedBy] [varchar](50) NULL,
    [IsEnabled][bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
) ON [PRIMARY]
GO

-- Create an index on the EmployeeNum column
CREATE INDEX IX_User_EmployeeNum
ON [dbo].[Users]([EmployeeNum])
GO
