CREATE PROC [dbo].[_org_willowcreek_CareCenter_UpdateVisitStatus]
	  @WorkflowId int,
	  @WorkflowStatus nvarchar(100)
AS

BEGIN
	DECLARE @VisitId INT = ( SELECT TOP 1 VisitId FROM _org_willowcreek_CareCenter_VisitWorkflow WHERE WorkflowId = @WorkflowId )

	IF @VisitId IS NOT NULL
	BEGIN

		-- If there are no other workflows for this visit that haven't been completed
		IF NOT EXISTS (
			SELECT VisitId
			FROM _org_willowcreek_CareCenter_VisitWorkflow VW
			INNER JOIN Workflow W 
				ON W.Id = VW.WorkflowId
				AND W.Id <> @WorkflowId
				AND W.CompletedDateTime IS NULL
			WHERE VW.VisitId = @VisitId
		)
		BEGIN

			IF @WorkflowStatus = 'Complete' OR EXISTS (
				SELECT VisitId
				FROM _org_willowcreek_CareCenter_VisitWorkflow VW
				INNER JOIN Workflow W 
					ON W.Id = VW.WorkflowId
					AND W.CompletedDateTime IS NOT NULL
					AND W.[Status] = 'Complete'
				WHERE VW.VisitId = @VisitId	
			)
			BEGIN
				-- If this workflow or any other workflow for the visit was completed, mark the visit as completed
				UPDATE _org_willowcreek_CareCenter_Visit SET [Status] = 1
				WHERE Id = @VisitId
			END
			ELSE
			BEGIN
				-- Otherwise mark the visit as being cancelled
				UPDATE _org_willowcreek_CareCenter_Visit SET [Status] = 2
				WHERE Id = @VisitId
			END

		END

	END

END