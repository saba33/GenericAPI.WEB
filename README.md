                
აქ მოცემულია ბაზის ამსახველი ფოტო თუ როგორ გამოიყურება, ასევე მოცემულია სკრიპტები რომელიც გამოყენებულია ბაზაში თეიბლების შესაქმნელად, 
orleans კონფიგურირებულია ADO Net ზე, ასევე პროექტში გამოყენებულია entity framework რისი საშუალებითაც ხდება ბაზასთან მუშაობა ყველა მოდელთან, რომელიც
არ არის ტრანზაქციულობასთან დაკავშირებული.  

                    SQL Tables For Project: 

![image](https://github.com/saba33/GenericAPI.WEB/assets/49198814/0a51ecb8-78dd-4945-9004-d3b34f4c02d0)

          Sql Scripts used for Crating Base Tables For orleans Project

                       SQL  Main Script 
DECLARE @current NVARCHAR(256);
DECLARE @snapshotSettings NVARCHAR(612);

SELECT @current = N'[' + (SELECT DB_NAME()) + N']';
SET @snapshotSettings = N'ALTER DATABASE ' + @current + N' SET READ_COMMITTED_SNAPSHOT ON; ALTER DATABASE ' + @current + N' SET ALLOW_SNAPSHOT_ISOLATION ON;';

EXECUTE sp_executesql @snapshotSettings;

-- This table defines Orleans operational queries. Orleans uses these to manage its operations,
-- these are the only queries Orleans issues to the database.
-- These can be redefined (e.g. to provide non-destructive updates) provided the stated interface principles hold.
CREATE TABLE OrleansQuery
(
	QueryKey VARCHAR(64) NOT NULL,
	QueryText VARCHAR(8000) NOT NULL,

	CONSTRAINT OrleansQuery_Key PRIMARY KEY(QueryKey)
);
-----------------------------------------------------------------------------------------------------------------------------------------
               SQLServer-Clustering.sql

CREATE TABLE OrleansMembershipVersionTable
(
	DeploymentId NVARCHAR(150) NOT NULL,
	Timestamp DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),
	Version INT NOT NULL DEFAULT 0,

	CONSTRAINT PK_OrleansMembershipVersionTable_DeploymentId PRIMARY KEY(DeploymentId)
);

-- Every silo instance has a row in the membership table.
CREATE TABLE OrleansMembershipTable
(
	DeploymentId NVARCHAR(150) NOT NULL,
	Address VARCHAR(45) NOT NULL,
	Port INT NOT NULL,
	Generation INT NOT NULL,
	SiloName NVARCHAR(150) NOT NULL,
	HostName NVARCHAR(150) NOT NULL,
	Status INT NOT NULL,
	ProxyPort INT NULL,
	SuspectTimes VARCHAR(8000) NULL,
	StartTime DATETIME2(3) NOT NULL,
	IAmAliveTime DATETIME2(3) NOT NULL,

	CONSTRAINT PK_MembershipTable_DeploymentId PRIMARY KEY(DeploymentId, Address, Port, Generation),
	CONSTRAINT FK_MembershipTable_MembershipVersionTable_DeploymentId FOREIGN KEY (DeploymentId) REFERENCES OrleansMembershipVersionTable (DeploymentId)
);

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'UpdateIAmAlivetimeKey','
	-- This is expected to never fail by Orleans, so return value
	-- is not needed nor is it checked.
	SET NOCOUNT ON;
	UPDATE OrleansMembershipTable
	SET
		IAmAliveTime = @IAmAliveTime
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'InsertMembershipVersionKey','
	SET NOCOUNT ON;
	INSERT INTO OrleansMembershipVersionTable
	(
		DeploymentId
	)
	SELECT @DeploymentId
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM
			OrleansMembershipVersionTable WITH(HOLDLOCK, XLOCK, ROWLOCK)
		WHERE
			DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
	);

	SELECT @@ROWCOUNT;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'InsertMembershipKey','
	SET XACT_ABORT, NOCOUNT ON;
	DECLARE @ROWCOUNT AS INT;
	BEGIN TRANSACTION;
	INSERT INTO OrleansMembershipTable
	(
		DeploymentId,
		Address,
		Port,
		Generation,
		SiloName,
		HostName,
		Status,
		ProxyPort,
		StartTime,
		IAmAliveTime
	)
	SELECT
		@DeploymentId,
		@Address,
		@Port,
		@Generation,
		@SiloName,
		@HostName,
		@Status,
		@ProxyPort,
		@StartTime,
		@IAmAliveTime
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM
			OrleansMembershipTable WITH(HOLDLOCK, XLOCK, ROWLOCK)
		WHERE
			DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
			AND Address = @Address AND @Address IS NOT NULL
			AND Port = @Port AND @Port IS NOT NULL
			AND Generation = @Generation AND @Generation IS NOT NULL
	);

	UPDATE OrleansMembershipVersionTable
	SET
		Timestamp = GETUTCDATE(),
		Version = Version + 1
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Version = @Version AND @Version IS NOT NULL
		AND @@ROWCOUNT > 0;

	SET @ROWCOUNT = @@ROWCOUNT;

	IF @ROWCOUNT = 0
		ROLLBACK TRANSACTION
	ELSE
		COMMIT TRANSACTION
	SELECT @ROWCOUNT;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'UpdateMembershipKey','
	SET XACT_ABORT, NOCOUNT ON;
	BEGIN TRANSACTION;

	UPDATE OrleansMembershipVersionTable
	SET
		Timestamp = GETUTCDATE(),
		Version = Version + 1
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Version = @Version AND @Version IS NOT NULL;

	UPDATE OrleansMembershipTable
	SET
		Status = @Status,
		SuspectTimes = @SuspectTimes,
		IAmAliveTime = @IAmAliveTime
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL
		AND @@ROWCOUNT > 0;

	SELECT @@ROWCOUNT;
	COMMIT TRANSACTION;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'GatewaysQueryKey','
	SELECT
		Address,
		ProxyPort,
		Generation
	FROM
		OrleansMembershipTable
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Status = @Status AND @Status IS NOT NULL
		AND ProxyPort > 0;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'MembershipReadRowKey','
	SELECT
		v.DeploymentId,
		m.Address,
		m.Port,
		m.Generation,
		m.SiloName,
		m.HostName,
		m.Status,
		m.ProxyPort,
		m.SuspectTimes,
		m.StartTime,
		m.IAmAliveTime,
		v.Version
	FROM
		OrleansMembershipVersionTable v
		-- This ensures the version table will returned even if there is no matching membership row.
		LEFT OUTER JOIN OrleansMembershipTable m ON v.DeploymentId = m.DeploymentId
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL
	WHERE
		v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'MembershipReadAllKey','
	SELECT
		v.DeploymentId,
		m.Address,
		m.Port,
		m.Generation,
		m.SiloName,
		m.HostName,
		m.Status,
		m.ProxyPort,
		m.SuspectTimes,
		m.StartTime,
		m.IAmAliveTime,
		v.Version
	FROM
		OrleansMembershipVersionTable v LEFT OUTER JOIN OrleansMembershipTable m
		ON v.DeploymentId = m.DeploymentId
	WHERE
		v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
	'DeleteMembershipTableEntriesKey','
	DELETE FROM OrleansMembershipTable
	WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
	DELETE FROM OrleansMembershipVersionTable
	WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
');

------------------------------------------------------------------------------------------------------------------------------------------


                      SQLServer-Persistence.sql

CREATE TABLE OrleansStorage
(
    -- These are for the book keeping. Orleans calculates
    -- these hashes (see RelationalStorageProvide implementation),
    -- which are signed 32 bit integers mapped to the *Hash fields.
    -- The mapping is done in the code. The
    -- *String columns contain the corresponding clear name fields.
    --
    -- If there are duplicates, they are resolved by using GrainIdN0,
    -- GrainIdN1, GrainIdExtensionString and GrainTypeString fields.
    -- It is assumed these would be rarely needed.
    GrainIdHash                INT NOT NULL,
    GrainIdN0                BIGINT NOT NULL,
    GrainIdN1                BIGINT NOT NULL,
    GrainTypeHash            INT NOT NULL,
    GrainTypeString            NVARCHAR(512) NOT NULL,
    GrainIdExtensionString    NVARCHAR(512) NULL,
    ServiceId                NVARCHAR(150) NOT NULL,

    -- Payload
    PayloadBinary    VARBINARY(MAX) NULL,

    -- Informational field, no other use.
    ModifiedOn DATETIME2(3) NOT NULL,

    -- The version of the stored payload.
    Version INT NULL

    -- The following would in principle be the primary key, but it would be too thick
    -- to be indexed, so the values are hashed and only collisions will be solved
    -- by using the fields. That is, after the indexed queries have pinpointed the right
    -- rows down to [0, n] relevant ones, n being the number of collided value pairs.
);

CREATE NONCLUSTERED INDEX IX_OrleansStorage ON OrleansStorage(GrainIdHash, GrainTypeHash);

-- This ensures lock escalation will not lock the whole table, which can potentially be enormous.
-- See more information at https://www.littlekendra.com/2016/02/04/why-rowlock-hints-can-make-queries-slower-and-blocking-worse-in-sql-server/.
ALTER TABLE OrleansStorage SET(LOCK_ESCALATION = DISABLE);

-- A feature with ID is compression. If it is supported, it is used for OrleansStorage table. This is an Enterprise feature.
-- This consumes more processor cycles, but should save on space on GrainIdString, GrainTypeString and ServiceId, which
-- contain mainly the same values. Also the payloads will be compressed.
IF EXISTS (SELECT 1 FROM sys.dm_db_persisted_sku_features WHERE feature_id = 100)
BEGIN
    ALTER TABLE OrleansStorage REBUILD PARTITION = ALL WITH(DATA_COMPRESSION = PAGE);
END

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
    'WriteToStorageKey',
    '-- When Orleans is running in normal, non-split state, there will
    -- be only one grain with the given ID and type combination only. This
    -- grain saves states mostly serially if Orleans guarantees are upheld. Even
    -- if not, the updates should work correctly due to version number.
    --
    -- In split brain situations there can be a situation where there are two or more
    -- grains with the given ID and type combination. When they try to INSERT
    -- concurrently, the table needs to be locked pessimistically before one of
    -- the grains gets @GrainStateVersion = 1 in return and the other grains will fail
    -- to update storage. The following arrangement is made to reduce locking in normal operation.
    --
    -- If the version number explicitly returned is still the same, Orleans interprets it so the update did not succeed
    -- and throws an InconsistentStateException.
    --
    -- See further information at https://docs.microsoft.com/dotnet/orleans/grains/grain-persistence.
    BEGIN TRANSACTION;
    SET XACT_ABORT, NOCOUNT ON;

    DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;


    -- If the @GrainStateVersion is not zero, this branch assumes it exists in this database.
    -- The NULL value is supplied by Orleans when the state is new.
    IF @GrainStateVersion IS NOT NULL
    BEGIN
        UPDATE OrleansStorage
        SET
            PayloadBinary = @PayloadBinary,
            ModifiedOn = GETUTCDATE(),
            Version = Version + 1,
            @NewGrainStateVersion = Version + 1,
            @GrainStateVersion = Version + 1
        WHERE
            GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
            AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
            AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
            AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
            AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
            AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
            AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
            AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
            OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));
    END

    -- The grain state has not been read. The following locks rather pessimistically
    -- to ensure only one INSERT succeeds.
    IF @GrainStateVersion IS NULL
    BEGIN
        INSERT INTO OrleansStorage
        (
            GrainIdHash,
            GrainIdN0,
            GrainIdN1,
            GrainTypeHash,
            GrainTypeString,
            GrainIdExtensionString,
            ServiceId,
            PayloadBinary,
            ModifiedOn,
            Version
        )
        SELECT
            @GrainIdHash,
            @GrainIdN0,
            @GrainIdN1,
            @GrainTypeHash,
            @GrainTypeString,
            @GrainIdExtensionString,
            @ServiceId,
            @PayloadBinary,
            GETUTCDATE(),
            1
         WHERE NOT EXISTS
         (
            -- There should not be any version of this grain state.
            SELECT 1
            FROM OrleansStorage WITH(XLOCK, ROWLOCK, HOLDLOCK, INDEX(IX_OrleansStorage))
            WHERE
                GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
                AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
                AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
                AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
                AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
                AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
                AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
         ) OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

        IF @@ROWCOUNT > 0
        BEGIN
            SET @NewGrainStateVersion = 1;
        END
    END

    SELECT @NewGrainStateVersion AS NewGrainStateVersion;
    COMMIT TRANSACTION;'
);

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
    'ClearStorageKey',
    'BEGIN TRANSACTION;
    SET XACT_ABORT, NOCOUNT ON;
    DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;
    UPDATE OrleansStorage
    SET
        PayloadBinary = NULL,
        ModifiedOn = GETUTCDATE(),
        Version = Version + 1,
        @NewGrainStateVersion = Version + 1
    WHERE
        GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
        AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
        AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
        AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
        AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
        AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
        AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
        OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

    SELECT @NewGrainStateVersion;
    COMMIT TRANSACTION;'
);

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
(
    'ReadFromStorageKey',
    '-- The application code will deserialize the relevant result. Not that the query optimizer
    -- estimates the result of rows based on its knowledge on the index. It does not know there
    -- will be only one row returned. Forcing the optimizer to process the first found row quickly
    -- creates an estimate for a one-row result and makes a difference on multi-million row tables.
    -- Also the optimizer is instructed to always use the same plan via index using the OPTIMIZE
    -- FOR UNKNOWN flags. These hints are only available in SQL Server 2008 and later. They
    -- should guarantee the execution time is robustly basically the same from query-to-query.
    SELECT
        PayloadBinary,
        Version
    FROM
        OrleansStorage
    WHERE
        GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
        AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
        AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
        AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
        AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
        AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
        AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));'
);
