SELECT 
  2 as Dimensions,
  p.SP2D_Name as ProfileName,
  l.GeometrySurface as LayerGeometry, 
  mpl.X as IntersectionX,
  null as Bottom,
  sum(case when mat.PN_Name = 'AbovePhreaticLevel' then mat.PV_Value end) AbovePhreaticLevel,
  sum(case when mat.PN_Name = 'BelowPhreaticLevel' then mat.PV_Value end) BelowPhreaticLevel,
  sum(case when mat.PN_Name = 'PermeabKx' then mat.PV_Value end) PermeabKx,
  sum(case when mat.PN_Name = 'DiameterD70' then mat.PV_Value end) DiameterD70,
  sum(case when mat.PN_Name = 'WhitesConstant' then mat.PV_Value end) WhitesConstant,
  sum(case when mat.PN_Name = 'BeddingAngle' then mat.PV_Value end) BeddingAngle
FROM MechanismPointLocation as m 
JOIN MechanismPointLocation as mpl ON p.SP2D_ID = mpl.SP2D_ID 
JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID 
JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID 
JOIN (
	SELECT m.MA_ID, pn.PN_Name, pv.PV_Value
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID
WHERE m.ME_ID = 4
GROUP BY l.SL2D_ID
UNION
SELECT 
  1 as Dimensions,
  p.SP1D_Name as ProfileName,
  null as LayerGeometry,
  null as IntersectionX,
  p.BottomLevel as Bottom,
  sum(case when mat.PN_Name = "AbovePhreaticLevel" then mat.PV_Value end) AbovePhreaticLevel,
  sum(case when mat.PN_Name = "BelowPhreaticLevel" then mat.PV_Value end) BelowPhreaticLevel,
  sum(case when mat.PN_Name = "PermeabKx" then mat.PV_Value end) PermeabKx,
  sum(case when mat.PN_Name = "DiameterD70" then mat.PV_Value end) DiameterD70,
  sum(case when mat.PN_Name = "WhitesConstant" then mat.PV_Value end) WhitesConstant,
  sum(case when mat.PN_Name = "BeddingAngle" then mat.PV_Value end) BeddingAngle
FROM SoilProfile1D as p
  JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID
  JOIN (
	SELECT m.MA_ID, pn.PN_Name, pv.PV_Value
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID) as mat ON l.MA_ID = mat.MA_ID
GROUP BY l.SL1D_ID

ORDER BY ProfileName