SELECT 
    2 as Dimension,
	p.SP2D_Name as ProfileName,
	lc.LayerCount,
	l.GeometrySurface as LayerGeometry, 
	mpl.X as IntersectionX,
	AbovePhreaticLevel,
	BelowPhreaticLevel,
	DryUnitWeight,
	IsAquifer
FROM Mechanism as m
JOIN MechanismPointLocation as mpl ON mpl.ME_ID = m.ME_ID
JOIN SoilProfile2D as p ON p.SP2D_ID = mpl.SP2D_ID
JOIN (
	SELECT SP2D_ID, COUNT(*) as LayerCount
	FROM SoilLayer2D
	GROUP BY SP2D_ID) lc ON  lc.SP2D_ID = p.SP2D_ID
JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID 
LEFT JOIN (
	SELECT 
		m.MA_ID, 
		sum(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) AbovePhreaticLevel,
		sum(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) BelowPhreaticLevel,
		sum(case when pn.PN_Name = 'DryUnitWeight' then pv.PV_Value end) DryUnitWeight
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID
	GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID
LEFT JOIN (
	SELECT 
		pv.SL2D_ID, 
		sum(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) IsAquifer
	FROM ParameterNames as pn
	JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID
	GROUP BY pv.SL2D_ID) as lpv ON lpv.SL2D_ID = l.SL2D_ID
WHERE m.ME_Name = "Piping"
ORDER BY ProfileName