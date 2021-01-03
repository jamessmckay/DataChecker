CREATE TABLE HangFire.AggregatedCounter (
    Key nvarchar(100) NOT NULL,
    Value bigint NOT NULL,
    ExpireAt datetime NULL,
    CONSTRAINT PK_HangFire_CounterAggregated PRIMARY KEY CLUSTERED (Key ASC)
);

GO
/****** Object:  Table HangFire.Counter    Script Date: 12/25/2020 6:05:55 PM ******/
CREATE TABLE HangFire.Counter(
    Key nvarchar(100) NOT NULL,
    Value int NOT NULL,
    ExpireAt datetime NULL
);

GO
/****** Object:  Table HangFire.Hash    Script Date: 12/25/2020 6:05:55 PM ******/
CREATE TABLE HangFire.Hash(
    Key nvarchar(100) NOT NULL,
    Field nvarchar(100) NOT NULL,
    Value nvarchar(max) NULL,
    ExpireAt datetime2(7) NULL,
    CONSTRAINT PK_HangFire_Hash PRIMARY KEY CLUSTERED (Key, Field)
);
GO

CREATE TABLE HangFire.Job (
    Id bigint IDENTITY(1,1) NOT NULL,
    StateId bigint NULL,
    StateName nvarchar(20) NULL,
    InvocationData nvarchar(max) NOT NULL,
    Arguments nvarchar(max) NOT NULL,
    CreatedAt datetime NOT NULL,
    ExpireAt datetime NULL,
    CONSTRAINT PK_HangFire_Job PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE TABLE HangFire.JobParameter (
    JobId bigint NOT NULL,
    Name nvarchar(40) NOT NULL,
    Value nvarchar(max) NULL,
    CONSTRAINT PK_HangFire_JobParameter PRIMARY KEY CLUSTERED (JobId, Name)
);
GO

CREATE TABLE HangFire.JobQueue (
    Id int IDENTITY(1,1) NOT NULL,
    JobId bigint NOT NULL,
    Queue nvarchar(50) NOT NULL,
    FetchedAt datetime NULL,
    CONSTRAINT PK_HangFire_JobQueue PRIMARY KEY CLUSTERED (Queue, Id)
);
GO

CREATE TABLE HangFire.List(
    Id bigint IDENTITY(1,1) NOT NULL,
    Key nvarchar(100) NOT NULL,
    Value nvarchar(max) NULL,
    ExpireAt datetime NULL,
    CONSTRAINT PK_HangFire_List PRIMARY KEY CLUSTERED (Key, Id)
);
GO

CREATE TABLE HangFire.Schema(
    Version int NOT NULL,
    CONSTRAINT PK_HangFire_Schema PRIMARY KEY CLUSTERED (Version)
);
GO

CREATE TABLE HangFire.Server(
    Id nvarchar(200) NOT NULL,
    Data nvarchar(max) NULL,
    LastHeartbeat datetime NOT NULL,
    CONSTRAINT PK_HangFire_Server PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE TABLE HangFire.Set (
    Key nvarchar(100) NOT NULL,
    Score float NOT NULL,
    Value nvarchar(256) NOT NULL,
    ExpireAt datetime NULL,
    CONSTRAINT PK_HangFire_Set PRIMARY KEY CLUSTERED (Key, Value)
);
GO

CREATE TABLE HangFire.State (
    Id bigint IDENTITY(1,1) NOT NULL,
    JobId bigint NOT NULL,
    Name nvarchar(20) NOT NULL,
    Reason nvarchar(100) NULL,
    CreatedAt datetime NOT NULL,
    Data nvarchar(max) NULL,
    CONSTRAINT PK_HangFire_State PRIMARY KEY CLUSTERED (JobId, Id)
);
GO
