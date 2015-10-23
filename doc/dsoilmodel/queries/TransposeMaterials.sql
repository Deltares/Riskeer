SELECT
    sl.SL2D_ID as Id,
    sl.MA_ID as Material,
    sum(case when properties.param = "AbovePhreaticLevel" then properties.val end) AbovePhreaticLevel,
    sum(case when properties.param = "BelowPhreaticLevel" then properties.val end) BelowPhreaticLevel,
	sum(case when properties.param = "PermeabKx" then properties.val end) PermeabKx,
	sum(case when properties.param = "DiameterD70" then properties.val end) DiameterD70,
	sum(case when properties.param = "WhitesConstant" then properties.val end) WhitesConstant,
	sum(case when properties.param = "BeddingAngle" then properties.val end) BeddingAngle
FROM SoilLayer2D as sl
JOIN (
	SELECT m.MA_ID as material, pn.PN_Name as param, pv.PV_Value as val
	FROM ParameterNames as pn
	JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
	JOIN Materials as m ON m.MA_ID = pv.MA_ID) as properties ON sl.MA_ID = properties.material
group by sl.MA_ID