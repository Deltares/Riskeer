SELECT 
  p.SP2D_Name,
  l.GeometrySurface, 
  mat.MA_ID, 
  mpl.X,
  sum(case when mat.PN_Name = "AbovePhreaticLevel" then mat.PV_Value end) AbovePhreaticLevel,
  sum(case when mat.PN_Name = "BelowPhreaticLevel" then mat.PV_Value end) BelowPhreaticLevel,
  sum(case when mat.PN_Name = "DryUnitWeight" then mat.PV_Value end) PermeabKx,
  sum(case when mat.PN_Name = "Distribution" then mat.Distribution end) Distribution
FROM MechanismPointLocation as m 
JOIN MechanismPointLocation as mpl ON p.SP2D_ID = mpl.SP2D_ID 
JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID 
JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID 
JOIN (
	SELECT m.MA_ID, pn.PN_NAme, pv.PV_Value, s.ST_Dist_Type as Distribution, s.ST_Mean as Mean, s.ST_Deviation as Deviation 
	FROM ParameterNames as pn
	LEFT JOIN Stochast as s ON s.PN_ID = pn.PN_ID
	LEFT JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID OR m.MA_ID = s.MA_ID) as mat ON l.MA_ID = mat.MA_ID
WHERE m.ME_ID = 4
GROUP BY l.SL2D_ID
ORDER BY p.SP2D_ID, l.SL2D_ID