/****** Object:  Table [dbo].[AnotherTempPersistent]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnotherTempPersistent](
	[AnotherTempPersistentId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[TempPersistentBaseId] [int] NULL,
 CONSTRAINT [PK_AnotherTempPersistent] PRIMARY KEY CLUSTERED 
(
	[AnotherTempPersistentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Item]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemType]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemType](
	[ItemTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ItemType] PRIMARY KEY CLUSTERED 
(
	[ItemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTypeToItem]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTypeToItem](
	[ItemTypeToItemId] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[ItemTypeId] [int] NOT NULL,
 CONSTRAINT [PK_ItemTypeToItem] PRIMARY KEY CLUSTERED 
(
	[ItemTypeToItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MainTestItem]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MainTestItem](
	[MainTestItemId] [int] IDENTITY(1,1) NOT NULL,
	[GuidNotNull] [uniqueidentifier] NOT NULL,
	[GuidNullable] [uniqueidentifier] NULL,
	[StringNullable] [nvarchar](50) NULL,
	[OrderIndex] [nvarchar](50) NOT NULL,
	[ParentId] [int] NULL,
 CONSTRAINT [PK_MainTestItem] PRIMARY KEY CLUSTERED 
(
	[MainTestItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TempPersistent]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempPersistent](
	[TempPersistentId] [int] IDENTITY(1,1) NOT NULL,
	[Caption] [nvarchar](max) NULL,
	[TempPersistentBaseId] [int] NULL,
 CONSTRAINT [PK_TempPersistent] PRIMARY KEY CLUSTERED 
(
	[TempPersistentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TempPersistentAlone]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempPersistentAlone](
	[TempPersistentAloneId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IntNullable] [int] NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK_TempPersistentAlone] PRIMARY KEY CLUSTERED 
(
	[TempPersistentAloneId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TempPersistentBase]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempPersistentBase](
	[TempPersistentBaseId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[TempPersistentAloneId] [int] NULL,
 CONSTRAINT [PK_TempPersistentBase] PRIMARY KEY CLUSTERED 
(
	[TempPersistentBaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ThirdLevelTempPersistent]    Script Date: 17.02.2013 0:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThirdLevelTempPersistent](
	[ThirdLevelTempPersistentId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[TempPersistentBaseId] [int] NULL,
 CONSTRAINT [PK_ThirdLevelTempPersistent] PRIMARY KEY CLUSTERED 
(
	[ThirdLevelTempPersistentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[AnotherTempPersistent]  WITH CHECK ADD  CONSTRAINT [FK_AnotherTempPersistent_TempPersistentBase] FOREIGN KEY([TempPersistentBaseId])
REFERENCES [dbo].[TempPersistentBase] ([TempPersistentBaseId])
GO
ALTER TABLE [dbo].[AnotherTempPersistent] CHECK CONSTRAINT [FK_AnotherTempPersistent_TempPersistentBase]
GO
ALTER TABLE [dbo].[ItemTypeToItem]  WITH CHECK ADD  CONSTRAINT [FK_ItemTypeToItem_Item] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Item] ([ItemId])
GO
ALTER TABLE [dbo].[ItemTypeToItem] CHECK CONSTRAINT [FK_ItemTypeToItem_Item]
GO
ALTER TABLE [dbo].[ItemTypeToItem]  WITH CHECK ADD  CONSTRAINT [FK_ItemTypeToItem_ItemType] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemType] ([ItemTypeId])
GO
ALTER TABLE [dbo].[ItemTypeToItem] CHECK CONSTRAINT [FK_ItemTypeToItem_ItemType]
GO
ALTER TABLE [dbo].[MainTestItem]  WITH CHECK ADD  CONSTRAINT [FK_MainTestItem_MainTestItem] FOREIGN KEY([ParentId])
REFERENCES [dbo].[MainTestItem] ([MainTestItemId])
GO
ALTER TABLE [dbo].[MainTestItem] CHECK CONSTRAINT [FK_MainTestItem_MainTestItem]
GO
ALTER TABLE [dbo].[TempPersistent]  WITH CHECK ADD  CONSTRAINT [FK_TempPersistent_TempPersistentBase] FOREIGN KEY([TempPersistentBaseId])
REFERENCES [dbo].[TempPersistentBase] ([TempPersistentBaseId])
GO
ALTER TABLE [dbo].[TempPersistent] CHECK CONSTRAINT [FK_TempPersistent_TempPersistentBase]
GO
ALTER TABLE [dbo].[TempPersistentBase]  WITH CHECK ADD  CONSTRAINT [FK_TempPersistentBase_TempPersistentAlone] FOREIGN KEY([TempPersistentAloneId])
REFERENCES [dbo].[TempPersistentAlone] ([TempPersistentAloneId])
GO
ALTER TABLE [dbo].[TempPersistentBase] CHECK CONSTRAINT [FK_TempPersistentBase_TempPersistentAlone]
GO
ALTER TABLE [dbo].[ThirdLevelTempPersistent]  WITH CHECK ADD  CONSTRAINT [FK_ThirdLevelTempPersistent_TempPersistentBase] FOREIGN KEY([TempPersistentBaseId])
REFERENCES [dbo].[TempPersistentBase] ([TempPersistentBaseId])
GO
ALTER TABLE [dbo].[ThirdLevelTempPersistent] CHECK CONSTRAINT [FK_ThirdLevelTempPersistent_TempPersistentBase]
GO
