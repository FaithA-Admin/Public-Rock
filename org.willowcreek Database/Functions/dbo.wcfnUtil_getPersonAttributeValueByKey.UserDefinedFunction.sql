/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_getPersonAttributeValueByKey]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnUtil_getPersonAttributeValueByKey]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_getPersonAttributeValueByKey]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[wcfnUtil_getPersonAttributeValueByKey](@Key VARCHAR(50), @Id INT, @Format VARCHAR(10) = NULL, @Key2 VARCHAR(50) = NULL, @Key3 VARCHAR(50) = NULL)
RETURNS NVARCHAR(max) 
AS 
BEGIN
	DECLARE @value NVARCHAR(max) = NULL
	DECLARE @value2 NVARCHAR(max) = NULL

	SELECT @value = av.Value
	FROM Attribute a
	JOIN AttributeValue av ON a.Id = av.AttributeId
									 AND av.EntityId = @Id
	WHERE a.EntityTypeId = 15
	AND a.[Key] = @Key

	IF @Key2 IS NOT NULL
		SELECT @value2 = av.Value
		FROM Attribute a
		JOIN AttributeValue av ON a.Id = av.AttributeId
										 AND av.EntityId = @Id
		WHERE a.EntityTypeId = 15
		AND a.[Key] = @Key2

	IF @Format = 'DATE'
		SET @value = CONVERT(VARCHAR(10), CAST(CASE @value WHEN '' THEN NULL ELSE @value END AS DATE), 101)

	IF @Format = 'FILE/DATE'
	BEGIN
		SET @value = CONVERT(VARCHAR(10), CAST(CASE @value WHEN '' THEN NULL ELSE @value END AS DATE), 101)
		IF @value2 IS NOT NULL
			IF @value IS NOT NULL
			BEGIN
				SET @value = '<a href=''/GetFile.ashx?guid=' + @value2 + ''' target="_blank">' 
					       + CONVERT(VARCHAR(10), CAST(CASE @value WHEN '' THEN NULL ELSE @value END AS DATE), 101) + '</a>'
			END
			ELSE
			BEGIN
				SET @value = '<a href=''/GetFile.ashx?guid=' + @value2 + ''' target="_blank">Review</a>'
			END
		ELSE IF @Key3 IS NOT NULL
		BEGIN
		--If no date yet get the latest workflow status for this applicant and workflow (Only doing BC in waiting for protection as is slower with all)
			SET @value = (SELECT '<a href=''/WorkflowEntry/' + CAST(WorkflowTypeId AS NVARCHAR(MAX)) + '/' + CAST(id AS NVARCHAR(MAX)) + ''' target="_blank">' + [Status] + '</a>' FROM (
				SELECT w.Id, wt.Name AS TypeName, WorkflowTypeId, w.Name, [Status], LastProcessedDateTime, ROW_NUMBER() OVER (PARTITION BY WorkflowTypeId ORDER By LastProcessedDateTime desc) AS rn
				FROM Workflow w
				JOIN WorkflowType wt ON wt.Id = WorkflowTypeId
				WHERE wt.Name NOT LIKE '%03%'
				AND REPLACE(wt.Name,' ','') LIKE '%' + @Key3 + '%'
				AND w.Id IN (
					SELECT av.EntityId
					FROM Attribute a
					JOIN AttributeValue av ON a.Id = av.AttributeId
					JOIN PersonAlias pa ON pa.PersonId = @Id
					WHERE a.EntityTypeId = 113 --Workflow
					AND a.Name LIKE '%Applicant%' 
					AND av.Value = CAST(pa.[Guid] AS NVARCHAR(MAX))
				AND [Status] LIKE '%Protection%'
				)
			) data 
			WHERE rn = 1
			)
		END
	END
	
	IF @Format = 'REVIEW'
	BEGIN
		SET @value = CONVERT(VARCHAR(10), CAST(CASE @value WHEN '' THEN NULL ELSE @value END AS DATE), 101)
		IF @value2 IS NOT NULL
			IF @value IS NOT NULL
			BEGIN
				SET @value = '' + CONVERT(VARCHAR(10), CAST(CASE @value WHEN '' THEN NULL ELSE @value END AS DATE), 101) + ''
			END
			ELSE
			BEGIN
				SET @value = 'Review'
			END
	END	

	RETURN COALESCE(@value,'')
END

GO
