rem set PROJ_LIB="D:\src\delft-tools1\src\apps\DelftShellPlugins\SharpMapGis\data\nad"
rem set GDAL_DATA="c:\gdal\data"
del rivers_rdnew.*
rem ogr2ogr -t_srs "+proj=sterea +lat_0=52.15616055555555 +lon_0=5.38763888888889 +k=0.9999079 +x_0=155000 +y_0=463000 +ellps=bessel +units=m +no_defs" rivers_rdnew.shp rivers.shp 
ogr2ogr -t_srs EPSG:28992 rivers_rdnew.shp rivers.shp 