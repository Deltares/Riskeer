SELECT 
    1 as Dimension,
	p.SP1D_Name as ProfileName,
	lc.LayerCount,
	p.BottomLevel as Bottom,
	l.TopLevel as Top,
	AbovePhreaticLevel,
	BelowPhreaticLevel,
	DryUnitWeight,
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
		max(case when pn.PN_Name = 'AbovePhreaticLevel' then pv.PV_Value end) AbovePhreaticLevel,
		max(case when pn.PN_Name = 'BelowPhreaticLevel' then pv.PV_Value end) BelowPhreaticLevel,
		max(case when pn.PN_Name = 'DryUnitWeight' then pv.PV_Value end) DryUnitWeight
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID
	GROUP BY m.MA_ID) as mat ON l.MA_ID = mat.MA_ID
LEFT JOIN (
	SELECT 
		pv.SL1D_ID, 
		max(case when pn.PN_Name = 'IsAquifer' then pv.PV_Value end) IsAquifer
	FROM ParameterNames as pn
	JOIN LayerParameterValues as pv ON pn.PN_ID = pv.PN_ID
	GROUP BY pv.SL1D_ID) as lpv ON lpv.SL1D_ID = l.SL1D_ID
ORDER BY ProfileName