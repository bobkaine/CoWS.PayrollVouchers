USE [master]
GO
/****** Object:  Database [CSVPDB]    Script Date: 10/9/2019 10:27:18 AM ******/
CREATE DATABASE [CSVPDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CSVPDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\CSVPDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CSVPDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\CSVPDB_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [CSVPDB] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CSVPDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CSVPDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CSVPDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CSVPDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CSVPDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CSVPDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [CSVPDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CSVPDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CSVPDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CSVPDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CSVPDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CSVPDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CSVPDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CSVPDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CSVPDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CSVPDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CSVPDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CSVPDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CSVPDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CSVPDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CSVPDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CSVPDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CSVPDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CSVPDB] SET RECOVERY FULL 
GO
ALTER DATABASE [CSVPDB] SET  MULTI_USER 
GO
ALTER DATABASE [CSVPDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CSVPDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CSVPDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CSVPDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CSVPDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'CSVPDB', N'ON'
GO
ALTER DATABASE [CSVPDB] SET QUERY_STORE = OFF
GO
USE [CSVPDB]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [CSVPDB]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](256) NULL,
	[LastName] [nvarchar](256) NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBatchHeader]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBatchHeader](
	[BatchHeaderID] [int] IDENTITY(1,1) NOT NULL,
	[BatchID] [varchar](50) NULL,
	[BatchType] [char](2) NOT NULL,
	[BatchTotal] [decimal](13, 2) NOT NULL,
	[FileDescription1] [varchar](50) NOT NULL,
	[FileDescription2] [varchar](50) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[Ledger] [varchar](5) NOT NULL,
	[Period] [varchar](2) NOT NULL,
	[Year] [varchar](4) NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
	[APApprover] [nvarchar](128) NULL,
	[GLApprover] [nvarchar](128) NULL,
	[FinalApprover] [nvarchar](128) NULL,
 CONSTRAINT [PK_tblBatchHeader] PRIMARY KEY CLUSTERED 
(
	[BatchHeaderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBatchHeaderAudit]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBatchHeaderAudit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[BatchHeaderID] [int] NOT NULL,
	[BatchID] [varchar](50) NULL,
	[BatchType] [char](2) NOT NULL,
	[BatchTotal] [decimal](13, 2) NOT NULL,
	[FileDescription1] [varchar](50) NOT NULL,
	[FileDescription2] [varchar](50) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[Ledger] [varchar](5) NULL,
	[Period] [varchar](2) NULL,
	[Year] [varchar](4) NULL,
	[APApprover] [nvarchar](128) NULL,
	[GLApprover] [nvarchar](128) NULL,
	[FinalApprover] [nvarchar](128) NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblBatchHeaderAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDocumentNumber]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDocumentNumber](
	[DocumentID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [char](2) NOT NULL,
	[PayPeriodEndingDate] [datetime] NOT NULL,
	[VendorName] [varchar](33) NOT NULL,
	[VendorNumber] [varchar](16) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[DocumentCreationDate] [datetime] NOT NULL,
	[PaymentType] [varchar](4) NOT NULL,
	[PaymentStatus] [char](2) NOT NULL,
	[CheckDescription] [varchar](28) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[BatchHeaderID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblDocumentNumber] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDocumentNumberAudit]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDocumentNumberAudit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[DocumentType] [char](2) NOT NULL,
	[PayPeriodEndingDate] [datetime] NOT NULL,
	[VendorName] [varchar](33) NOT NULL,
	[VendorNumber] [varchar](16) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[DocumentCreationDate] [datetime] NOT NULL,
	[PaymentType] [varchar](4) NOT NULL,
	[PaymentStatus] [char](2) NOT NULL,
	[CheckDescription] [varchar](28) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[BatchHeaderID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblDocumentNumberAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGeneralLedger]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGeneralLedger](
	[GLID] [int] IDENTITY(1,1) NOT NULL,
	[GLType] [char](1) NOT NULL,
	[BatchNumber] [varchar](50) NULL,
	[BatchType] [varchar](2) NOT NULL,
	[BatchTotal] [decimal](13, 2) NOT NULL,
	[GLComments] [varchar](35) NOT NULL,
	[JournalComments] [varchar](35) NOT NULL,
	[Ledger] [varchar](5) NOT NULL,
	[Period] [varchar](2) NOT NULL,
	[Year] [varchar](4) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblGeneralLedger] PRIMARY KEY CLUSTERED 
(
	[GLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGeneralLedgerAudit]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGeneralLedgerAudit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[GLID] [int] NOT NULL,
	[GLType] [char](1) NOT NULL,
	[BatchNumber] [varchar](50) NULL,
	[BatchType] [varchar](2) NOT NULL,
	[BatchTotal] [decimal](13, 2) NOT NULL,
	[GLComments] [varchar](35) NOT NULL,
	[JournalComments] [varchar](35) NOT NULL,
	[Ledger] [varchar](5) NOT NULL,
	[Period] [varchar](2) NOT NULL,
	[Year] [varchar](4) NOT NULL,
	[UserCode] [varchar](3) NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblGeneralLedgerAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGeneralSettings]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGeneralSettings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[ChildSupportDesc] [varchar](80) NOT NULL,
	[FilePath] [varchar](50) NOT NULL,
	[UserCode] [varchar](3) NULL,
	[VouchersPayableDesc] [varchar](80) NOT NULL,
	[TempPassword] [varchar](25) NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NULL,
 CONSTRAINT [PK_tblConfig_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGLTransactionDetail]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGLTransactionDetail](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionAmount] [decimal](13, 2) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[Comments] [varchar](35) NOT NULL,
	[Fund] [varchar](3) NOT NULL,
	[ResponsibilityCode] [varchar](6) NOT NULL,
	[ObjectCode] [varchar](6) NOT NULL,
	[ProgramCode] [varchar](6) NOT NULL,
	[GLID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblGLTransactionDetail] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGLTransactionDetailAudit]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGLTransactionDetailAudit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionID] [int] NOT NULL,
	[TransactionAmount] [decimal](13, 2) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[Comments] [varchar](35) NOT NULL,
	[Fund] [varchar](3) NOT NULL,
	[ResponsibilityCode] [varchar](6) NOT NULL,
	[ObjectCode] [varchar](6) NOT NULL,
	[ProgramCode] [varchar](6) NOT NULL,
	[GLID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblGLTransactionDetailAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSoapXML]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSoapXML](
	[BatchType] [varchar](2) NOT NULL,
	[ID] [int] NOT NULL,
	[Request] [xml] NOT NULL,
	[Response] [xml] NOT NULL,
	[DatePosted] [datetime] NOT NULL,
 CONSTRAINT [PK_tblSoapXML] PRIMARY KEY CLUSTERED 
(
	[BatchType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTransactionDetail]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTransactionDetail](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[TransCode] [varchar](3) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CheckAmount] [decimal](13, 2) NOT NULL,
	[ResponsibilityCode] [varchar](6) NOT NULL,
	[ObjectCode] [varchar](6) NOT NULL,
	[ProgramCode] [varchar](6) NOT NULL,
	[LineItemDescription] [varchar](13) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblTransactionDetail] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTransactionDetailAudit]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTransactionDetailAudit](
	[AuditID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionID] [int] NOT NULL,
	[TransCode] [varchar](3) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CheckAmount] [decimal](13, 2) NOT NULL,
	[ResponsibilityCode] [varchar](6) NOT NULL,
	[ObjectCode] [varchar](6) NOT NULL,
	[ProgramCode] [varchar](6) NOT NULL,
	[LineItemDescription] [varchar](13) NOT NULL,
	[BankNumber] [varchar](4) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[WhoModified] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_tblTransactionDetailAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblWebServiceSettings]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblWebServiceSettings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FMSUser] [varchar](50) NOT NULL,
	[FMSPassword1] [varchar](50) NOT NULL,
	[FMSPassword2] [varchar](50) NULL,
	[FMSPassword3] [varchar](50) NULL,
	[Ledger] [varchar](50) NOT NULL,
	[OSUser] [varchar](50) NOT NULL,
	[OSPassword] [varchar](50) NOT NULL,
	[OutputDevice] [varchar](50) NOT NULL,
	[WebServiceURL] [varchar](100) NULL,
	[DateModified] [date] NULL,
	[WhoModified] [nvarchar](128) NULL,
 CONSTRAINT [PK_tblFMSConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleId]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblDocumentNumber]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblDocumentNumber] ON [dbo].[tblDocumentNumber]
(
	[DocumentCreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblSoapResponse]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblSoapResponse] ON [dbo].[tblSoapXML]
(
	[BatchType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblTransactionDetail]    Script Date: 10/9/2019 10:27:19 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblTransactionDetail] ON [dbo].[tblTransactionDetail]
(
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[tblDocumentNumber]  WITH CHECK ADD  CONSTRAINT [FK_tblDocumentNumber_tblBatchHeader] FOREIGN KEY([BatchHeaderID])
REFERENCES [dbo].[tblBatchHeader] ([BatchHeaderID])
GO
ALTER TABLE [dbo].[tblDocumentNumber] CHECK CONSTRAINT [FK_tblDocumentNumber_tblBatchHeader]
GO
ALTER TABLE [dbo].[tblGLTransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_tblGLTransactionDetail_tblGeneralLedger] FOREIGN KEY([GLID])
REFERENCES [dbo].[tblGeneralLedger] ([GLID])
GO
ALTER TABLE [dbo].[tblGLTransactionDetail] CHECK CONSTRAINT [FK_tblGLTransactionDetail_tblGeneralLedger]
GO
ALTER TABLE [dbo].[tblTransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_tblTransactionDetail_tblDocumentNumber] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[tblDocumentNumber] ([DocumentID])
GO
ALTER TABLE [dbo].[tblTransactionDetail] CHECK CONSTRAINT [FK_tblTransactionDetail_tblDocumentNumber]
GO
/****** Object:  StoredProcedure [dbo].[sel_PreliminarySummaryRpt]    Script Date: 10/9/2019 10:27:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bob Kaine
-- Create date: 2019-09-14
-- Description:	Preliminary Summary Report (CS, PV)
-- =============================================
CREATE PROCEDURE [dbo].[sel_PreliminarySummaryRpt] 
	@CreateDate date,
	@BatchType varchar(2)
AS
BEGIN
	SELECT  bh.BatchHeaderID,
			bh.FileDescription1,
			bh.FileDescription2,
			bh.BatchType,
			bh.BatchTotal,
			bh.BankNumber,
			bh.UserCode,
			bh.[Period],
			bh.[Year],
			dn.CheckDescription,
			dn.DocumentCreationDate,
			dn.PaymentStatus,
			dn.PaymentType,
			dn.PayPeriodEndingDate,
			dn.VendorName,
			dn.VendorNumber,
			td.CreationDate as CheckCreationDate,
			td.LineItemDescription,
			td.CheckAmount,
			td.ResponsibilityCode,
			td.ObjectCode,
			td.ProgramCode,
			td.TransCode
			FROM dbo.tblBatchHeader bh
	INNER JOIN dbo.tblDocumentNumber dn
	ON bh.BatchHeaderID = dn.BatchHeaderID
	INNER JOIN dbo.tblTransactionDetail td
	ON dn.DocumentID = td.DocumentID
	WHERE dn.DocumentCreationDate = @CreateDate
	AND bh.BatchType = @BatchType
END
GO
USE [master]
GO
ALTER DATABASE [CSVPDB] SET  READ_WRITE 
GO
