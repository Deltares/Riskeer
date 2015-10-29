SELECT 
    1 as Dimension,
	p.SP1D_Name as ProfileName,
	lc.LayerCount,
	p.BottomLevel as Bottom,
	l.TopLevel as Top,
	AbovePhreaticLevel,
	BelowPhreaticLevel,
	PermeabKx,
	DiameterD70,
	WhitesConstant,
	BeddingAngle,
	IsAquifer
FROM SoilProfile1D as p
JOIN (
	SELECT SP1D_ID, COUNT(*) as LayerCount
	FROM SoilLayer1D
	GROUP BY SP1D_ID) lc ON  lc.SP1D_ID = p.SP1D_ID
JOIN SoilLayer1D as l ON l.SP1D_ID = p.SP1D_ID
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
		pv.SL1D_ID, 
		sum(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) IsAquifer
	FROM ParameterNames as pn
	JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID
	GROUP BY pv.SL1D_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID
ORDER BY ProfileName