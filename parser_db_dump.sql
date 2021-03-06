USE [master]
GO
/****** Object:  Database [parsingForDev]    Script Date: 16.09.2020 15:24:09 ******/
CREATE DATABASE [parsingForDev]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'parsing', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\parsing.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'parsing_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\parsing_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [parsingForDev] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [parsingForDev].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [parsingForDev] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [parsingForDev] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [parsingForDev] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [parsingForDev] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [parsingForDev] SET ARITHABORT OFF 
GO
ALTER DATABASE [parsingForDev] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [parsingForDev] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [parsingForDev] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [parsingForDev] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [parsingForDev] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [parsingForDev] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [parsingForDev] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [parsingForDev] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [parsingForDev] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [parsingForDev] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [parsingForDev] SET  DISABLE_BROKER 
GO
ALTER DATABASE [parsingForDev] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [parsingForDev] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [parsingForDev] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [parsingForDev] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [parsingForDev] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [parsingForDev] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [parsingForDev] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [parsingForDev] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [parsingForDev] SET  MULTI_USER 
GO
ALTER DATABASE [parsingForDev] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [parsingForDev] SET DB_CHAINING OFF 
GO
ALTER DATABASE [parsingForDev] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [parsingForDev] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [parsingForDev]
GO
/****** Object:  Table [dbo].[brand]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[brand](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_brand] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[category]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[url] [nvarchar](max) NOT NULL,
	[product_amount] [int] NULL,
 CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[error]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[error](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[url] [nvarchar](max) NOT NULL,
	[error_source_id] [int] NOT NULL,
	[code] [int] NULL,
	[message] [nvarchar](max) NULL,
 CONSTRAINT [PK_error] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[page_type]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[page_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_page_type] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[product]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_id] [int] NOT NULL,
	[url] [nvarchar](max) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[price] [float] NOT NULL,
	[article] [nvarchar](max) NULL,
	[description] [nvarchar](max) NULL,
	[brand_id] [int] NULL,
 CONSTRAINT [PK_product] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[product_file]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_file](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[name] [nvarchar](max) NULL,
	[url] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_product_image] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[property]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[property](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[name_id] [int] NOT NULL,
	[value_id] [int] NOT NULL,
 CONSTRAINT [PK_property] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[property_name]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[property_name](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_property_name] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[property_value]    Script Date: 16.09.2020 15:24:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[property_value](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_property_value] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[error]  WITH CHECK ADD  CONSTRAINT [FK_error_page_type] FOREIGN KEY([error_source_id])
REFERENCES [dbo].[page_type] ([id])
GO
ALTER TABLE [dbo].[error] CHECK CONSTRAINT [FK_error_page_type]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK_product_brand] FOREIGN KEY([brand_id])
REFERENCES [dbo].[brand] ([id])
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK_product_brand]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK_product_category] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK_product_category]
GO
ALTER TABLE [dbo].[product_file]  WITH CHECK ADD  CONSTRAINT [FK_product_file_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[product_file] CHECK CONSTRAINT [FK_product_file_product]
GO
ALTER TABLE [dbo].[property]  WITH CHECK ADD  CONSTRAINT [FK_property_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[property] CHECK CONSTRAINT [FK_property_product]
GO
ALTER TABLE [dbo].[property]  WITH CHECK ADD  CONSTRAINT [FK_property_property_name] FOREIGN KEY([name_id])
REFERENCES [dbo].[property_name] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[property] CHECK CONSTRAINT [FK_property_property_name]
GO
ALTER TABLE [dbo].[property]  WITH CHECK ADD  CONSTRAINT [FK_property_property_value] FOREIGN KEY([value_id])
REFERENCES [dbo].[property_value] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[property] CHECK CONSTRAINT [FK_property_property_value]
GO
USE [master]
GO
ALTER DATABASE [parsingForDev] SET  READ_WRITE 
GO
