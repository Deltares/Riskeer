SELECT 
    2 as Dimension,
	p.SP2D_Name as ProfileName,
	lc.LayerCount,
	l.GeometrySurface as LayerGeometry, 
	mpl.X as IntersectionX,
	AbovePhreaticLevel,
	BelowPhreaticLevel,
	PermeabKx,
	DiameterD70,
	WhitesConstant,
	BeddingAngle,
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
		sum(case when pn.PN_Name = 'PermeabKx' then pv.PV_Value end) PermeabKx,
		sum(case when pn.PN_Name = 'DiameterD70' then pv.PV_Value end) DiameterD70,
		sum(case when pn.PN_Name = 'WhitesConstant' then pv.PV_Value end) WhitesConstant,
		sum(case when pn.PN_Name = 'BeddingAngle' then pv.PV_Value end) BeddingAngle
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID
	GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID
JOIN (
	SELECT 
		pv.SL2D_ID, 
		sum(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) IsAquifer
	FROM ParameterNames as pn
	JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID
	GROUP BY pv.SL2D_ID) as lpv ON lpv.SL2D_ID = l.SL2D_ID
WHERE m.ME_Name = "Piping"
ORDER BY ProfileName