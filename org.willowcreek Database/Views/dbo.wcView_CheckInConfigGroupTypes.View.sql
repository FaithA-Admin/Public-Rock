create view CheckInConfigGroupTypes as
		with Grade as (
			select GradeID = AREA.ID, GradeName = AREA.Name, GradeOrder = AREA.[Order], AREA.ID, AREA.Name, Level = 1, ConfigName = CFG.Name, ConfigID = CFG.ID
			from GroupType CFG
			join GroupTypeAssociation GTA on GTA.GroupTypeId = CFG.Id
			join GroupType AREA on GTA.ChildGroupTypeId = AREA.Id
			where CFG.GroupTypePurposeValueId = 142
			union all
			select Grade.GradeID, Grade.GradeName, Grade.GradeOrder, GT.ID, GT.Name, Level = Level + 1, ConfigName, ConfigID
			from Grade 
			join GroupTypeAssociation GTA on Grade.ID = GTA.GroupTypeId
			join GroupType GT on GTA.ChildGroupTypeId = GT.Id
		)
		select GroupTypeId = ID, ConfigID 
		from Grade
