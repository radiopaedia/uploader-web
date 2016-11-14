USE [Radiopaedia]
GO
CREATE TABLE [dbo].[ApiClient](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[site_id] [varchar](max) NULL,
	[site_secret] [varchar](max) NULL,
	[redirect_url] [varchar](max) NULL,
	[oauth_url] [varchar](max) NULL,
	[cases_url] [varchar](max) NULL,
	[users_url] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Cases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[case_id] [varchar](max) NULL,
	[username] [varchar](max) NULL,
	[date] [datetime] NULL,
	[status] [varchar](max) NULL,
	[status_message] [varchar](max) NULL,
	[title] [varchar](max) NULL,
	[system_id] [int] NULL,
	[diagnostic_certainty_id] [int] NULL,
	[suitable_for_quiz] [bit] NULL,
	[presentation] [varchar](max) NULL,
	[age] [int] NULL,
	[body] [varchar](max) NULL,
	[r_case_id] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Events](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [datetime] NULL,
	[Type] [varchar](max) NULL,
	[InternalId] [varchar](max) NULL,
	[StudyUid] [varchar](max) NULL,
	[SeriesUid] [varchar](max) NULL,
	[Message] [varchar](max) NULL,
	[Data] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


CREATE TABLE [dbo].[PACS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocalAe] [varchar](max) NULL,
	[IP] [varchar](max) NULL,
	[Port] [int] NULL,
	[AET] [varchar](max) NULL,
	[Description] [varchar](max) NULL,
	[Selected] [bit] NULL,
	[Notes] [varchar](max) NULL,
	[LocalStorage] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Series](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[case_id] [varchar](max) NULL,
	[study_uid] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[images] [int] NULL,
	[series_uid] [varchar](max) NULL,
	[crop_x] [int] NULL,
	[crop_y] [int] NULL,
	[crop_h] [int] NULL,
	[crop_w] [int] NULL,
	[window_wc] [int] NULL,
	[window_ww] [nchar](10) NULL,
	[every_image] [int] NULL,
	[start_image] [int] NULL,
	[end_image] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Studies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[case_id] [varchar](max) NULL,
	[study_uid] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[date] [datetime] NULL,
	[modality] [varchar](max) NULL,
	[findings] [varchar](max) NULL,
	[caption] [varchar](max) NULL,
	[images] [int] NULL,
	[position] [int] NULL,
	[r_study_id] [varchar](max) NULL,
	[patient_id] [varchar](max) NULL,
	[patient_name] [varchar](max) NULL,
	[patient_dob] [date] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](max) NULL,
	[access_token] [varchar](max) NULL,
	[refresh_token] [varchar](max) NULL,
	[expiry_date] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO