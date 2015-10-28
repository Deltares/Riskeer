SELECT 
  p.SP1D_Name as ProfileName,
  p.BottomLevel as Bottom,
  l.TopLevel as Top,
  sum(case when lpv.PN_Name = 'IsAquifer' then lpv.PV_Value end) IsAquifer,
  sum(case when mat.PN_Name = 'AbovePhreaticLevel' then mat.PV_Value end) AbovePhreaticLevel,
  sum(case when mat.PN_Name = 'BelowPhreaticLevel' then mat.PV_Value end) BelowPhreaticLevel,
  sum(case when mat.PN_Name = 'PermeabKx' then mat.PV_Value end) PermeabKx,
  sum(case when mat.PN_Name = 'DiameterD70' then mat.PV_Value end) DiameterD70,
  sum(case when mat.PN_Name = 'WhitesConstant' then mat.PV_Value end) WhitesConstant,
  sum(case when mat.PN_Name = 'BeddingAngle' then mat.PV_Value end) BeddingAngle
FROM SoilProfile1D as p
  JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID
  JOIN (
	SELECT m.MA_ID, pn.PN_Name, pv.PV_Value
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID
  JOIN (
	SELECT pv.SL1D_ID, pn.PN_Name, pv.PV_Value
	FROM ParameterNames as pn
	JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID
GROUP BY l.SL1D_ID

ORDER BY ProfileName