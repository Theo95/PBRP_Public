USE [master]
GO
/****** Object:  Database [pb-rp]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE DATABASE [pb-rp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'pb-rp', FILENAME = N'S:\SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\pb-rp.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'pb-rp_log', FILENAME = N'S:\SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\pb-rp_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [pb-rp] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [pb-rp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [pb-rp] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [pb-rp] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [pb-rp] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [pb-rp] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [pb-rp] SET ARITHABORT OFF 
GO
ALTER DATABASE [pb-rp] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [pb-rp] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [pb-rp] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [pb-rp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [pb-rp] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [pb-rp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [pb-rp] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [pb-rp] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [pb-rp] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [pb-rp] SET  DISABLE_BROKER 
GO
ALTER DATABASE [pb-rp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [pb-rp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [pb-rp] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [pb-rp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [pb-rp] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [pb-rp] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [pb-rp] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [pb-rp] SET RECOVERY FULL 
GO
ALTER DATABASE [pb-rp] SET  MULTI_USER 
GO
ALTER DATABASE [pb-rp] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [pb-rp] SET DB_CHAINING OFF 
GO
ALTER DATABASE [pb-rp] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [pb-rp] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [pb-rp] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'pb-rp', N'ON'
GO
ALTER DATABASE [pb-rp] SET QUERY_STORE = OFF
GO
USE [pb-rp]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [pb-rp]
GO
/****** Object:  User [smallyhome]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [smallyhome] FOR LOGIN [smallyhome] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [PB-RP\William.Young]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [PB-RP\William.Young] FOR LOGIN [PB-RP\William.Young] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [PB-RP\Web]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [PB-RP\Web] FOR LOGIN [PB-RP\Web] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [PB-RP\game]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [PB-RP\game] FOR LOGIN [PB-RP\game] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [PB-RP\donovan]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [PB-RP\donovan] FOR LOGIN [PB-RP\donovan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [pbrp]    Script Date: 8/7/2017 9:59:46 PM ******/
CREATE USER [pbrp] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [smallyhome]
GO
ALTER ROLE [db_owner] ADD MEMBER [PB-RP\William.Young]
GO
ALTER ROLE [db_owner] ADD MEMBER [PB-RP\Web]
GO
ALTER ROLE [db_owner] ADD MEMBER [PB-RP\game]
GO
ALTER ROLE [db_owner] ADD MEMBER [PB-RP\donovan]
GO
ALTER ROLE [db_owner] ADD MEMBER [pbrp]
GO
/****** Object:  Table [dbo].[adminrecord]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adminrecord](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[PunishingAdmin] [int] NOT NULL,
	[PunishedPlayer] [int] NOT NULL,
	[Reason] [varchar](50) NULL,
	[PunishedMasterAccount] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [adminrecord_id_pk] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[article_comments]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[article_comments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[target] [int] NOT NULL,
	[text] [varchar](max) NOT NULL,
	[author] [int] NOT NULL,
	[reported] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[articles]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[articles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](512) NOT NULL,
	[hasShortStory] [int] NOT NULL,
	[shortStory] [varchar](max) NULL,
	[body] [varchar](max) NOT NULL,
	[published] [int] NOT NULL,
	[allowComments] [int] NULL,
	[author] [int] NOT NULL,
	[slug] [varchar](1024) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
 CONSTRAINT [PK_articles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[bankaccount]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bankaccount](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[RegisterOwnerId] [int] NOT NULL,
	[Balance] [bigint] NOT NULL,
	[Pin] [varchar](64) NOT NULL,
	[Locked] [bit] NOT NULL,
	[CardNumber] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[FailedPinAttempts] [int] NOT NULL,
	[LockedType] [int] NOT NULL,
 CONSTRAINT [PK_bankaccount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[bans]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bans](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[BannedId] [int] NOT NULL,
	[BannedPlayer] [int] NOT NULL,
	[BannedBy] [int] NOT NULL,
	[BannedByPlayer] [int] NOT NULL,
	[BannedUntil] [datetime] NOT NULL,
	[BanReason] [varchar](64) NULL,
 CONSTRAINT [PK_bans] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[changelogs]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[changelogs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NOT NULL,
	[version] [varchar](50) NOT NULL,
	[body] [varchar](max) NOT NULL,
	[author] [int] NOT NULL,
	[published] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
	[updateBy] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[email_tokens]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[email_tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[token] [varchar](50) NOT NULL,
	[used] [int] NOT NULL,
	[maID] [int] NOT NULL,
	[created_at] [datetime2](3) NOT NULL,
	[updated_at] [datetime2](3) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[factions]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[factions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[Type] [int] NOT NULL,
	[MaxRank] [int] NOT NULL,
	[ManageRank] [int] NOT NULL,
	[Bank] [int] NOT NULL,
	[RadioFrequency] [int] NOT NULL,
	[FactionChatDisabled] [bit] NOT NULL,
 CONSTRAINT [PK_factions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[faq]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[faq](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Question] [nvarchar](255) NOT NULL,
	[Answer] [nvarchar](255) NOT NULL,
	[Published] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[gasstations]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[gasstations](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[CurrentPetrol] [int] NOT NULL,
	[PetrolPrice] [int] NOT NULL,
	[MaxPetrol] [int] NOT NULL,
	[CurrentDiesel] [int] NOT NULL,
	[DieselPrice] [int] NOT NULL,
	[MaxDiesel] [int] NOT NULL,
	[RefillAreaX1] [float] NOT NULL,
	[RefillAreaY1] [float] NOT NULL,
	[RefillAreaZ1] [float] NOT NULL,
	[RefillAreaR1] [float] NOT NULL,
	[RefillAreaX2] [float] NOT NULL,
	[RefillAreaY2] [float] NOT NULL,
	[RefillAreaZ2] [float] NOT NULL,
	[RefillAreaR2] [float] NOT NULL,
 CONSTRAINT [PK_gasstations] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[inventory]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[inventory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[OwnerType] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Name] [varchar](32) NOT NULL,
	[Value] [varchar](64) NOT NULL,
	[Quantity] [int] NOT NULL,
	[VehicleId] [int] NOT NULL,
	[PropertyId] [int] NOT NULL,
	[HouseId] [int] NOT NULL,
	[BusinessId] [int] NOT NULL,
	[DroppedX] [float] NOT NULL,
	[DroppedY] [float] NOT NULL,
	[DroppedZ] [float] NOT NULL,
	[DroppedRX] [float] NOT NULL,
	[DroppedRY] [float] NOT NULL,
	[DroppedRZ] [float] NOT NULL,
	[DroppedDimension] [int] NOT NULL,
	[SlotPosition] [int] NULL,
 CONSTRAINT [PK_inventory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[licenses]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[licenses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_licenses] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[masters]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[masters](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](32) NOT NULL,
	[Status] [int] NOT NULL,
	[Email] [varchar](64) NULL,
	[Password] [varchar](129) NOT NULL,
	[RegistrationIP] [varchar](20) NULL,
	[LatestIP] [varchar](20) NULL,
	[LatestLogin] [datetime] NULL,
	[AdminLevel] [int] NOT NULL,
	[Developer] [int] NULL,
	[Tester] [int] NULL,
	[HasDevAccess] [bit] NOT NULL,
	[FailedLoginAttempts] [int] NOT NULL,
	[KeyCursor] [int] NOT NULL,
	[KeyInventory] [int] NOT NULL,
	[KeyInteract] [int] NOT NULL,
	[PerformanceSetting] [int] NOT NULL,
	[AccountLocked] [bit] NOT NULL,
	[SocialClubName] [varchar](50) NULL,
	[email_confirmed] [int] NULL,
	[remember_token] [varchar](256) NULL,
	[AvatarPath] [varchar](50) NULL,
	[ReApplyTime] [int] NOT NULL,
	[quiz_at] [datetime] NULL,
	[created_at] [datetime2](3) NULL,
	[updated_at] [datetime2](3) NULL,
	[ForumProfile] [varchar](100) NULL,
	[RegisteredBetaInterest] [int] NOT NULL,
 CONSTRAINT [PK_masters] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mdc_apb]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mdc_apb](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ApbType] [int] NOT NULL,
	[PlacedByPlayerId] [int] NOT NULL,
	[Target] [varchar](200) NULL,
	[Description] [varchar](max) NULL,
	[Cleared] [bit] NULL,
	[TimeSubmitted] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mdc_apbcomments]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mdc_apbcomments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NULL,
	[ApdId] [int] NULL,
	[Comment] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mdc_callresponses]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mdc_callresponses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[CallId] [int] NULL,
	[UnitName] [varchar](45) NULL,
	[UnitCleared] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mdc_comments]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mdc_comments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[CallId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[UnitName] [varchar](50) NULL,
	[Timestamp] [datetime] NOT NULL,
	[Comment] [varchar](500) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mdc_emscalls]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mdc_emscalls](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[CallerId] [int] NULL,
	[CallTime] [datetime] NULL,
	[RefferedTo] [int] NULL,
	[CallerNameGiven] [varchar](100) NULL,
	[CallDescription] [varchar](500) NULL,
	[CallStatus] [int] NULL,
	[CallType] [int] NULL,
	[CallLocationX] [float] NULL,
	[CallLocationY] [float] NULL,
	[CallLocationZ] [float] NULL,
	[PhoneNumber] [varchar](11) NULL,
	[CallLocationName] [varchar](100) NULL,
	[CallLocationStreetName] [varchar](150) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[migrations]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[migrations](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[migration] [nvarchar](255) NOT NULL,
	[batch] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notifications]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notifications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](255) NOT NULL,
	[Recipient] [int] NOT NULL,
	[Author] [int] NOT NULL,
	[Subject] [varchar](255) NOT NULL,
	[Body] [varchar](max) NOT NULL,
	[Read] [bit] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[password_resets]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[password_resets](
	[email] [nvarchar](255) NOT NULL,
	[token] [nvarchar](255) NOT NULL,
	[created_at] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pdlog]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pdlog](
	[id] [nchar](10) NOT NULL,
	[itemID] [nchar](10) NOT NULL,
	[logType] [nchar](10) NOT NULL,
	[dateOut] [date] NULL,
	[dateIn] [date] NULL,
	[timeOut] [time](7) NULL,
	[timeIn] [time](7) NULL,
	[signeeName] [nvarchar](50) NULL,
	[productName] [nvarchar](50) NULL,
	[serial] [nvarchar](50) NULL,
 CONSTRAINT [PK_pdlog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phoneapps]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phoneapps](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[AppId] [int] NOT NULL,
	[PhoneId] [int] NOT NULL,
	[Purchased] [bit] NOT NULL,
	[Installed] [bit] NOT NULL,
	[Position] [varchar](4) NOT NULL,
 CONSTRAINT [PK_phoneapps] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phonecontacts]    Script Date: 8/7/2017 9:59:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phonecontacts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Number] [varchar](50) NULL,
	[Address1] [varchar](64) NULL,
	[Address2] [varchar](50) NULL,
	[Address3] [varchar](50) NULL,
	[Notes] [varchar](64) NULL,
	[IsFavourite] [bit] NOT NULL,
	[IsBlocked] [bit] NOT NULL,
	[IsSimContact] [bit] NOT NULL,
	[SavedTo] [int] NOT NULL,
 CONSTRAINT [PK_phonecontacts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phonelog]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phonelog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[IMEIFrom] [bigint] NULL,
	[IMEITo] [bigint] NULL,
	[NumberFrom] [varchar](20) NULL,
	[NumberTo] [varchar](20) NULL,
	[Type] [int] NOT NULL,
	[Duration] [int] NULL,
	[SentAt] [datetime] NULL,
	[Message] [varchar](128) NULL,
	[Viewed] [bit] NOT NULL,
	[DeletedFrom] [bit] NOT NULL,
	[DeletedTo] [bit] NOT NULL,
 CONSTRAINT [PK_phonelog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phones]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phones](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[IMEI] [bigint] NOT NULL,
	[InstalledSim] [int] NOT NULL,
	[BatteryLevel] [int] NOT NULL,
	[ContactCapacity] [int] NOT NULL,
	[PoweredOn] [bit] NOT NULL,
	[Muted] [bit] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[WallpaperId] [int] NOT NULL,
	[Passcode] [varchar](128) NULL,
	[PassActive] [bit] NOT NULL,
 CONSTRAINT [PK_phones] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[players]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[players](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](32) NOT NULL,
	[MasterId] [int] NOT NULL,
	[FactionId] [int] NOT NULL,
	[FactionRank] [int] NOT NULL,
	[WeaponSkillData] [varchar](64) NOT NULL,
	[Level] [int] NOT NULL,
	[Money] [bigint] NOT NULL,
	[Gender] [int] NOT NULL,
	[DateOfBirth] [datetime] NULL,
	[Health] [float] NOT NULL,
	[Armour] [float] NOT NULL,
	[Downed] [bit] NOT NULL,
	[BloodLevel] [float] NOT NULL,
	[StabWounds] [int] NOT NULL,
	[BulletWounds] [int] NOT NULL,
	[MeleeHits] [int] NOT NULL,
	[LastPosX] [float] NOT NULL,
	[LastPosY] [float] NOT NULL,
	[LastPosZ] [float] NOT NULL,
	[LastRotX] [float] NOT NULL,
	[LastRotY] [float] NOT NULL,
	[LastRotZ] [float] NOT NULL,
	[Dimension] [int] NOT NULL,
	[Skin] [int] NOT NULL,
	[TestDriveBan] [bit] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
	[CurrentlyConnected] [bit] NULL,
 CONSTRAINT [PK_players] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[prison]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[prison](
	[id] [int] NOT NULL,
	[characterID] [int] NOT NULL,
	[time] [int] NOT NULL,
	[isPrison] [bit] NOT NULL,
	[cellNum] [int] NOT NULL,
	[jailorID] [int] NOT NULL,
 CONSTRAINT [PK_prison] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[properties]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[properties](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[SubType] [int] NOT NULL,
	[OwnerType] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Key] [varchar](128) NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[EnterX] [float] NOT NULL,
	[EnterY] [float] NOT NULL,
	[EnterZ] [float] NOT NULL,
	[IsEnterable] [bit] NOT NULL,
	[ExitX] [float] NOT NULL,
	[ExitY] [float] NOT NULL,
	[ExitZ] [float] NOT NULL,
	[Dimension] [int] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[Interior] [int] NOT NULL,
	[ExitRZ] [float] NOT NULL,
 CONSTRAINT [PK_properties] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[quiz]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quiz](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[MasterAccount] [int] NOT NULL,
	[Result] [int] NULL,
	[Correct] [int] NULL,
	[created_at] [datetime2](3) NULL,
	[updated_at] [datetime2](3) NULL,
 CONSTRAINT [PK__quiz__3213E83FD0694274] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[quiz_applications]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quiz_applications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [int] NOT NULL,
	[MasterAccount] [int] NOT NULL,
	[Question1Answer] [varchar](max) NOT NULL,
	[Question2Answer] [varchar](max) NOT NULL,
	[Handler] [int] NOT NULL,
	[HandlerNotes] [varchar](512) NULL,
	[created_at] [datetime2](3) NOT NULL,
	[updated_at] [datetime2](3) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[quiz_questions]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quiz_questions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Question] [varchar](1024) NOT NULL,
	[Type] [int] NOT NULL,
	[Answer1] [varchar](2048) NOT NULL,
	[Answer2] [varchar](2048) NOT NULL,
	[Answer3] [varchar](2048) NOT NULL,
	[Answer4] [varchar](2048) NOT NULL,
	[CorrectAnswer] [int] NOT NULL,
 CONSTRAINT [quiz_questions_id_pk] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ranks]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ranks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[FactionId] [int] NOT NULL,
	[Title] [varchar](64) NOT NULL,
	[CanFire] [bit] NOT NULL,
	[CanHire] [bit] NOT NULL,
	[CanPromote] [bit] NOT NULL,
	[CanDemote] [bit] NOT NULL,
	[CanFrespawn] [bit] NOT NULL,
	[CanToggleFactionChat] [bit] NOT NULL,
	[RepositionVehicles] [bit] NOT NULL,
	[IsLeader] [bit] NOT NULL,
	[OrderIndex] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[referrals]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[referrals](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ReferrerId] [int] NOT NULL,
	[ReferredId] [int] NOT NULL,
	[Valid] [int] NULL,
	[created_at] [datetime2](3) NOT NULL,
	[updated_at] [datetime2](3) NOT NULL,
	[processed_at] [datetime2](3) NULL,
 CONSTRAINT [PK__referral__3213E83F42FB2CA7] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[server]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[server](
	[DateTime] [datetime] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [server_id_pk] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[shopitems]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[shopitems](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[BusinessId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Quantity] [int] NULL,
	[Price] [int] NULL,
 CONSTRAINT [PK_shopitems] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[simcards]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[simcards](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [varchar](20) NULL,
	[Credit] [bigint] NOT NULL,
	[IsBlocked] [bit] NOT NULL,
	[ContactCapacity] [int] NOT NULL,
 CONSTRAINT [PK_simcards] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skins]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[skins](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Model] [int] NOT NULL,
	[DrawableIds] [varchar](32) NOT NULL,
	[TextureIds] [varchar](32) NOT NULL,
 CONSTRAINT [PK_skins] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ticket_categories]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ticket_categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[AdminLevel] [int] NOT NULL,
	[DeveloperLevel] [int] NOT NULL,
	[TesterLevel] [int] NOT NULL,
	[Active] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ticket_replies]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ticket_replies](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[Author] [int] NOT NULL,
	[StaffReply] [int] NOT NULL,
	[Body] [nvarchar](255) NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tickets]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tickets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Category] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[AssignedTo] [int] NULL,
	[Status] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tokens]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NULL,
	[Token] [int] NULL,
	[Type] [int] NULL,
	[Time] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[vehicles]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[vehicles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Model] [int] NOT NULL,
	[Color1] [varchar](7) NOT NULL,
	[Color2] [varchar](7) NOT NULL,
	[Dimension] [int] NOT NULL,
	[Health] [float] NOT NULL,
	[BodyHealth] [float] NOT NULL,
	[DirtLevel] [float] NOT NULL,
	[Key] [varchar](64) NULL,
	[KeyInIgnition] [bit] NOT NULL,
	[LicensePlate] [varchar](10) NOT NULL,
	[LicensePlateStyle] [int] NOT NULL,
	[EngineDamagePerSecond] [float] NOT NULL,
	[Fuel] [float] NOT NULL,
	[FactionId] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Locked] [bit] NOT NULL,
	[PosX] [float] NOT NULL,
	[PosY] [float] NOT NULL,
	[PosZ] [float] NOT NULL,
	[RotX] [float] NOT NULL,
	[RotY] [float] NOT NULL,
	[RotZ] [float] NOT NULL,
	[IsEngineOn] [bit] NOT NULL,
	[DoorData] [varchar](16) NULL,
	[WindowData] [varchar](16) NULL,
	[SidejobId] [int] NULL,
	[PaintFade] [float] NOT NULL,
	[RepairType] [int] NOT NULL,
	[RepairTime] [datetime2](7) NOT NULL,
	[RepairCost] [float] NOT NULL,
	[TyreData] [varchar](16) NOT NULL,
	[WindscreenDetached] [bit] NOT NULL,
 CONSTRAINT [PK_vehicles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[weapons]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[weapons](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Model] [int] NOT NULL,
	[Ammo] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ExteriorSerial] [varchar](128) NULL,
	[BarrelSerial] [varchar](128) NULL,
	[CurrentOwnerId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[DroppedAt] [datetime] NULL,
	[CreatedAt] [datetime] NULL,
	[DroppedX] [float] NOT NULL,
	[DroppedY] [float] NOT NULL,
	[DroppedZ] [float] NOT NULL,
	[DroppedRX] [float] NOT NULL,
	[DroppedRY] [float] NOT NULL,
	[DroppedRZ] [float] NOT NULL,
	[DroppedDimension] [int] NOT NULL,
 CONSTRAINT [PK_weapons] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[whitelist]    Script Date: 8/7/2017 9:59:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[whitelist](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ip] [varchar](16) NOT NULL,
	[amount] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Index [mdc_apb_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [mdc_apb_id_uindex] ON [dbo].[mdc_apb]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [mdc_apbcomments_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [mdc_apbcomments_id_uindex] ON [dbo].[mdc_apbcomments]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [mdc_callresponses_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [mdc_callresponses_id_uindex] ON [dbo].[mdc_callresponses]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [mdc_comments_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [mdc_comments_id_uindex] ON [dbo].[mdc_comments]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [emscalls_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [emscalls_id_uindex] ON [dbo].[mdc_emscalls]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [password_resets_email_index]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE NONCLUSTERED INDEX [password_resets_email_index] ON [dbo].[password_resets]
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [password_resets_token_index]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE NONCLUSTERED INDEX [password_resets_token_index] ON [dbo].[password_resets]
(
	[token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [tokens_id_uindex]    Script Date: 8/7/2017 9:59:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [tokens_id_uindex] ON [dbo].[tokens]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[adminrecord] ADD  CONSTRAINT [DF_adminrecord_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[adminrecord] ADD  CONSTRAINT [DF__adminreco__creat__76F68FE1]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[adminrecord] ADD  CONSTRAINT [DF__adminreco__updat__77EAB41A]  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[article_comments] ADD  CONSTRAINT [DF_article_comments_reported]  DEFAULT ((0)) FOR [reported]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_RegisterOwnerId]  DEFAULT ((-1)) FOR [RegisterOwnerId]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_Table_1_AccountBalance]  DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_Pin]  DEFAULT ('') FOR [Pin]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_Locked]  DEFAULT ((0)) FOR [Locked]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_CardNumber]  DEFAULT ((0)) FOR [CardNumber]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_FailedPinAttempts]  DEFAULT ((0)) FOR [FailedPinAttempts]
GO
ALTER TABLE [dbo].[bankaccount] ADD  CONSTRAINT [DF_bankaccount_LockedType]  DEFAULT ((-1)) FOR [LockedType]
GO
ALTER TABLE [dbo].[bans] ADD  CONSTRAINT [DF_bans_BannedPlayer]  DEFAULT ((0)) FOR [BannedPlayer]
GO
ALTER TABLE [dbo].[bans] ADD  CONSTRAINT [DF_bans_BannedByPlayer]  DEFAULT ((0)) FOR [BannedByPlayer]
GO
ALTER TABLE [dbo].[factions] ADD  CONSTRAINT [DF_factions_FactionChatDisabled]  DEFAULT ((0)) FOR [FactionChatDisabled]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_CurrentPetrol]  DEFAULT ((0)) FOR [CurrentPetrol]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_Table_1_CurrentPetrolPrice]  DEFAULT ((1)) FOR [PetrolPrice]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_MaxPetrol]  DEFAULT ((0)) FOR [MaxPetrol]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_CurrentDiesel]  DEFAULT ((0)) FOR [CurrentDiesel]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_DieselPrice]  DEFAULT ((0)) FOR [DieselPrice]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_MaxDiesel]  DEFAULT ((0)) FOR [MaxDiesel]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaX1]  DEFAULT ((0)) FOR [RefillAreaX1]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaY1]  DEFAULT ((0)) FOR [RefillAreaY1]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaZ1]  DEFAULT ((0)) FOR [RefillAreaZ1]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaR1]  DEFAULT ((0)) FOR [RefillAreaR1]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaX2]  DEFAULT ((0)) FOR [RefillAreaX2]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaY2]  DEFAULT ((0)) FOR [RefillAreaY2]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaZ2]  DEFAULT ((0)) FOR [RefillAreaZ2]
GO
ALTER TABLE [dbo].[gasstations] ADD  CONSTRAINT [DF_gasstations_RefillAreaR2]  DEFAULT ((0)) FOR [RefillAreaR2]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_OwnerId]  DEFAULT ((-1)) FOR [OwnerId]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_OwnerType]  DEFAULT ((1)) FOR [OwnerType]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_Type]  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_Name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_Value]  DEFAULT ('') FOR [Value]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_VehicleId]  DEFAULT ((-1)) FOR [VehicleId]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_PropertyId]  DEFAULT ((-1)) FOR [PropertyId]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_HouseId]  DEFAULT ((-1)) FOR [HouseId]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_BusinessId]  DEFAULT ((-1)) FOR [BusinessId]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedX]  DEFAULT ((0)) FOR [DroppedX]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedY]  DEFAULT ((0)) FOR [DroppedY]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedZ]  DEFAULT ((0)) FOR [DroppedZ]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedRX]  DEFAULT ((0)) FOR [DroppedRX]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedRY]  DEFAULT ((0)) FOR [DroppedRY]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedRZ]  DEFAULT ((0)) FOR [DroppedRZ]
GO
ALTER TABLE [dbo].[inventory] ADD  CONSTRAINT [DF_inventory_DroppedDimension]  DEFAULT ((0)) FOR [DroppedDimension]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [masters_LatestLogin_default]  DEFAULT (getdate()) FOR [LatestLogin]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_AdminLevel]  DEFAULT ((0)) FOR [AdminLevel]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [masters_Developer_default]  DEFAULT ((0)) FOR [Developer]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [masters_Tester_default]  DEFAULT ((0)) FOR [Tester]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_HasDevAccess]  DEFAULT ((0)) FOR [HasDevAccess]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_FailedLoginAttempts]  DEFAULT ((0)) FOR [FailedLoginAttempts]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_KeyCursor]  DEFAULT ((77)) FOR [KeyCursor]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_KeyInventory]  DEFAULT ((73)) FOR [KeyInventory]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_KeyInteract]  DEFAULT ((69)) FOR [KeyInteract]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_PerformanceSetting]  DEFAULT ((4)) FOR [PerformanceSetting]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_AccountLocked]  DEFAULT ((0)) FOR [AccountLocked]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_SocialClubName]  DEFAULT ('') FOR [SocialClubName]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_email_confirmed]  DEFAULT ((0)) FOR [email_confirmed]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_ReApplyTime]  DEFAULT ((0)) FOR [ReApplyTime]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF__masters__ForumPr__070CFC19]  DEFAULT (NULL) FOR [ForumProfile]
GO
ALTER TABLE [dbo].[masters] ADD  CONSTRAINT [DF_masters_RegisteredBetaInterest]  DEFAULT ((0)) FOR [RegisteredBetaInterest]
GO
ALTER TABLE [dbo].[phoneapps] ADD  CONSTRAINT [DF_phoneapps_AppId]  DEFAULT ((0)) FOR [AppId]
GO
ALTER TABLE [dbo].[phoneapps] ADD  CONSTRAINT [DF_phoneapps_PhoneId]  DEFAULT ((0)) FOR [PhoneId]
GO
ALTER TABLE [dbo].[phoneapps] ADD  CONSTRAINT [DF_phoneapps_Purchased]  DEFAULT ((0)) FOR [Purchased]
GO
ALTER TABLE [dbo].[phoneapps] ADD  CONSTRAINT [DF_phoneapps_Installed]  DEFAULT ((0)) FOR [Installed]
GO
ALTER TABLE [dbo].[phoneapps] ADD  CONSTRAINT [DF_phoneapps_Position]  DEFAULT ('0,0') FOR [Position]
GO
ALTER TABLE [dbo].[phonecontacts] ADD  CONSTRAINT [DF_phonecontacts_IsFavourite]  DEFAULT ((0)) FOR [IsFavourite]
GO
ALTER TABLE [dbo].[phonecontacts] ADD  CONSTRAINT [DF_phonecontacts_IsBlocked]  DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[phonecontacts] ADD  CONSTRAINT [DF_phonecontacts_IsSimContact]  DEFAULT ((0)) FOR [IsSimContact]
GO
ALTER TABLE [dbo].[phonecontacts] ADD  CONSTRAINT [DF_phonecontacts_SavedTo]  DEFAULT ((0)) FOR [SavedTo]
GO
ALTER TABLE [dbo].[phonelog] ADD  CONSTRAINT [DF_phonelog_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[phonelog] ADD  CONSTRAINT [DF_phonelog_Viewed]  DEFAULT ((0)) FOR [Viewed]
GO
ALTER TABLE [dbo].[phonelog] ADD  CONSTRAINT [DF_phonelog_DeletedFrom]  DEFAULT ((0)) FOR [DeletedFrom]
GO
ALTER TABLE [dbo].[phonelog] ADD  CONSTRAINT [DF_phonelog_DeletedTo]  DEFAULT ((0)) FOR [DeletedTo]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_RegisteredTo]  DEFAULT ((0)) FOR [IMEI]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_InstalledSim]  DEFAULT ((-1)) FOR [InstalledSim]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_BatteryLevel]  DEFAULT ((100)) FOR [BatteryLevel]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_PoweredOn]  DEFAULT ((1)) FOR [PoweredOn]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_Muted]  DEFAULT ((0)) FOR [Muted]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_IsPrimary]  DEFAULT ((0)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_WallpaperId]  DEFAULT ((0)) FOR [WallpaperId]
GO
ALTER TABLE [dbo].[phones] ADD  CONSTRAINT [DF_phones_PassActive]  DEFAULT ((0)) FOR [PassActive]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_MasterId]  DEFAULT ((0)) FOR [MasterId]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_FactionId]  DEFAULT ((0)) FOR [FactionId]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_FactionRank]  DEFAULT ((0)) FOR [FactionRank]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_WeaponSkillData]  DEFAULT ('0,0,0,0,0,0,0,0') FOR [WeaponSkillData]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Level]  DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Money]  DEFAULT ((0)) FOR [Money]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Gender]  DEFAULT ((0)) FOR [Gender]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Health]  DEFAULT ((100.0)) FOR [Health]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Armour]  DEFAULT ((0.0)) FOR [Armour]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Downed]  DEFAULT ((0)) FOR [Downed]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_TimeUntilDeath]  DEFAULT ((100.0)) FOR [BloodLevel]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_StabWounds]  DEFAULT ((0)) FOR [StabWounds]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_BulletWounds]  DEFAULT ((0)) FOR [BulletWounds]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_MeleeHits]  DEFAULT ((0)) FOR [MeleeHits]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_PosX]  DEFAULT ((0.0)) FOR [LastPosX]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_LastPosY]  DEFAULT ((0.0)) FOR [LastPosY]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_LastPosZ]  DEFAULT ((0.0)) FOR [LastPosZ]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_LastRotX]  DEFAULT ((0.0)) FOR [LastRotX]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_LastRotY]  DEFAULT ((0.0)) FOR [LastRotY]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_LastRotZ]  DEFAULT ((0.0)) FOR [LastRotZ]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Dimension]  DEFAULT ((0)) FOR [Dimension]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_Skin]  DEFAULT ((797459875)) FOR [Skin]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_TestDriveBan]  DEFAULT ((0)) FOR [TestDriveBan]
GO
ALTER TABLE [dbo].[players] ADD  CONSTRAINT [DF_players_updated_at]  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[players] ADD  DEFAULT ((0)) FOR [CurrentlyConnected]
GO
ALTER TABLE [dbo].[properties] ADD  CONSTRAINT [DF_properties_SubtypeId]  DEFAULT ((-1)) FOR [SubType]
GO
ALTER TABLE [dbo].[properties] ADD  CONSTRAINT [DF_properties_OwnerType]  DEFAULT ((-1)) FOR [OwnerType]
GO
ALTER TABLE [dbo].[properties] ADD  CONSTRAINT [DF_properties_IsLocked]  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[properties] ADD  CONSTRAINT [DF_properties_Interior]  DEFAULT ((-1)) FOR [Interior]
GO
ALTER TABLE [dbo].[properties] ADD  DEFAULT ((0)) FOR [ExitRZ]
GO
ALTER TABLE [dbo].[quiz_applications] ADD  CONSTRAINT [DF_quiz_applications_Handler]  DEFAULT ((0)) FOR [Handler]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_FactionId]  DEFAULT ((0)) FOR [FactionId]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanFire]  DEFAULT ((0)) FOR [CanFire]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanHire]  DEFAULT ((0)) FOR [CanHire]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanPromote]  DEFAULT ((0)) FOR [CanPromote]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanDemote]  DEFAULT ((0)) FOR [CanDemote]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanFrespawn]  DEFAULT ((0)) FOR [CanFrespawn]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_CanToggleFactionChat]  DEFAULT ((0)) FOR [CanToggleFactionChat]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_RepositionVehicles]  DEFAULT ((0)) FOR [RepositionVehicles]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_IsLeader]  DEFAULT ((0)) FOR [IsLeader]
GO
ALTER TABLE [dbo].[ranks] ADD  CONSTRAINT [DF_ranks_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[referrals] ADD  CONSTRAINT [DF__referrals__Valid__3D690CCA]  DEFAULT ((0)) FOR [Valid]
GO
ALTER TABLE [dbo].[server] ADD  DEFAULT ((0)) FOR [DateTime]
GO
ALTER TABLE [dbo].[shopitems] ADD  CONSTRAINT [DF_shopitems_BusinessId]  DEFAULT ((-1)) FOR [BusinessId]
GO
ALTER TABLE [dbo].[simcards] ADD  CONSTRAINT [DF_simcards_IsBlocked]  DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[simcards] ADD  CONSTRAINT [DF_simcards_ContactCapacity]  DEFAULT ((20)) FOR [ContactCapacity]
GO
ALTER TABLE [dbo].[skins] ADD  CONSTRAINT [DF_skins_OwnerId]  DEFAULT ((0)) FOR [OwnerId]
GO
ALTER TABLE [dbo].[skins] ADD  CONSTRAINT [DF_skins_Model]  DEFAULT ((0)) FOR [Model]
GO
ALTER TABLE [dbo].[skins] ADD  CONSTRAINT [DF_skins_DrawableIds]  DEFAULT ((0)) FOR [DrawableIds]
GO
ALTER TABLE [dbo].[skins] ADD  CONSTRAINT [DF_skins_TextureIds]  DEFAULT ((0)) FOR [TextureIds]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Color1]  DEFAULT ((0)) FOR [Color1]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Color2]  DEFAULT ((0)) FOR [Color2]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Dimension]  DEFAULT ((0)) FOR [Dimension]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Health]  DEFAULT ((1000)) FOR [Health]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_BodyHealth]  DEFAULT ((1000)) FOR [BodyHealth]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_DirtLevel]  DEFAULT ((0)) FOR [DirtLevel]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Key]  DEFAULT ('') FOR [Key]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_KeyInIgnition]  DEFAULT ((0)) FOR [KeyInIgnition]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_LicensePlate]  DEFAULT ('XYZ 001') FOR [LicensePlate]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_LicensePlateStyle]  DEFAULT ((1)) FOR [LicensePlateStyle]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_EngineDamagePerSecond]  DEFAULT ((0)) FOR [EngineDamagePerSecond]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Fuel]  DEFAULT ((100)) FOR [Fuel]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_FactionId]  DEFAULT ((-1)) FOR [FactionId]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_OwnerId]  DEFAULT ((-1)) FOR [OwnerId]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_Locked]  DEFAULT ((1)) FOR [Locked]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_PosX]  DEFAULT ((0.0)) FOR [PosX]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_PosY]  DEFAULT ((0.0)) FOR [PosY]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_PosZ]  DEFAULT ((0)) FOR [PosZ]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_RotX]  DEFAULT ((0.0)) FOR [RotX]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_RotY]  DEFAULT ((0.0)) FOR [RotY]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_RotZ]  DEFAULT ((0.0)) FOR [RotZ]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_IsEngineOn]  DEFAULT ((0)) FOR [IsEngineOn]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_DoorData]  DEFAULT ('0,0,0,0') FOR [DoorData]
GO
ALTER TABLE [dbo].[vehicles] ADD  CONSTRAINT [DF_vehicles_WindowData]  DEFAULT ('0,0,0,0') FOR [WindowData]
GO
ALTER TABLE [dbo].[vehicles] ADD  DEFAULT ((1)) FOR [PaintFade]
GO
ALTER TABLE [dbo].[vehicles] ADD  DEFAULT ((0)) FOR [RepairType]
GO
ALTER TABLE [dbo].[vehicles] ADD  DEFAULT ('0,0,0,0,0,0,0') FOR [TyreData]
GO
ALTER TABLE [dbo].[vehicles] ADD  DEFAULT ((0)) FOR [WindscreenDetached]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_Model]  DEFAULT ((0)) FOR [Model]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_Ammo]  DEFAULT ((0)) FOR [Ammo]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_CurrentOwnerId]  DEFAULT ((0)) FOR [CurrentOwnerId]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedX]  DEFAULT ((0.0)) FOR [DroppedX]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedY]  DEFAULT ((0.0)) FOR [DroppedY]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedZ]  DEFAULT ((0.0)) FOR [DroppedZ]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedRX]  DEFAULT ((0.0)) FOR [DroppedRX]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedRY]  DEFAULT ((0.0)) FOR [DroppedRY]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedRZ]  DEFAULT ((0.0)) FOR [DroppedRZ]
GO
ALTER TABLE [dbo].[weapons] ADD  CONSTRAINT [DF_weapons_DroppedDimension]  DEFAULT ((0)) FOR [DroppedDimension]
GO
USE [master]
GO
ALTER DATABASE [pb-rp] SET  READ_WRITE 
GO
