SELECT 
  p.SP2D_Name,
  l.GeometrySurface, 
  mat.MA_ID, 
  mpl.X,
  sum(case when mat.PN_Name = "AbovePhreaticLevel" then mat.PV_Value end) AbovePhreaticLevel,
  sum(case when mat.PN_Name = "BelowPhreaticLevel" then mat.PV_Value end) BelowPhreaticLevel,
  sum(case when mat.PN_Name = "PermeabKx" then mat.PV_Value end) PermeabKx,
  sum(case when mat.PN_Name = "DiameterD70" then mat.PV_Value end) DiameterD70,
  sum(case when mat.PN_Name = "WhitesConstant" then mat.PV_Value end) WhitesConstant,
  sum(case when mat.PN_Name = "BeddingAngle" then mat.PV_Value end) BeddingAngle
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
ORDER BY p.SP2D_ID, l.SL2D_ID