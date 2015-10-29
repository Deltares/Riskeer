 SELECT 
	m.MA_ID, 
    sum(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) AbovePhreaticLevel,
    sum(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) BelowPhreaticLevel,
    sum(case when pn.PN_Name = 'PermeabKx' then pv.PV_Value end) PermeabKx,
    sum(case when pn.PN_Name = 'DiameterD70' then pv.PV_Value end) DiameterD70,
    sum(case when pn.PN_Name = 'WhitesConstant' then pv.PV_Value end) WhitesConstant,
    sum(case when pn.PN_Name = 'BeddingAngle' then pv.PV_Value end) BeddingAngle
  FROM ParameterNames as pn
  JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
  JOIN Materials as m ON m.MA_ID = pv.MA_ID
  GROUP BY m.MA_ID