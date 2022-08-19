<?xml version="1.0" encoding="utf-8"?>
<uboi:Assembleren-collectie xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:gml="http://www.opengis.net/gml/3.2" xmlns:imwap="http://www.aquo.nl/BOI2023/imwaproxies/v20210113" gml:id="assemblage1" xmlns:uboi="http://www.aquo.nl/BOI2023/uitwisselmodel/v20210113">
  <uboi:featureMember>
    <imwap:Waterkeringstelsel gml:id="section1">
      <imwap:naam>Traject A</imwap:naam>
      <imwap:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 100 0</gml:posList>
        </gml:LineString>
      </imwap:geometrie2D>
      <imwap:lengte>100</imwap:lengte>
      <imwap:typeWaterkeringstelsel>Dijktraject</imwap:typeWaterkeringstelsel>
    </imwap:Waterkeringstelsel>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Beoordelingsproces gml:id="beoordelingsproces1">
      <uboi:beginJaarBeoordelingsronde>2023</uboi:beginJaarBeoordelingsronde>
      <uboi:eindJaarBeoordelingsronde>2035</uboi:eindJaarBeoordelingsronde>
      <uboi:beoordeelt xlink:href="section1" />
    </uboi:Beoordelingsproces>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Veiligheidsoordeel gml:id="veiligheidsoordeel1">
      <uboi:categorie>B</uboi:categorie>
      <uboi:assemblagemethodeVeiligheidsoordeel>BOI-2B-1</uboi:assemblagemethodeVeiligheidsoordeel>
      <uboi:faalkans>0.00068354</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-2A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:uitkomstVan xlink:href="beoordelingsproces1" />
    </uboi:Veiligheidsoordeel>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="toetsspoorGABI">
      <uboi:faalkans>0.08419</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="veiligheidsoordeel1" />
      <uboi:generiekFaalmechanisme>GABI</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:SpecifiekFaalmechanisme gml:id="specifiekFaalmechanisme">
      <uboi:faalkans>0.002834</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="veiligheidsoordeel1" />
      <uboi:specifiekFaalmechanisme>Specifiek faalmechanisme</uboi:specifiekFaalmechanisme>
    </uboi:SpecifiekFaalmechanisme>
  </uboi:featureMember>
</uboi:Assembleren-collectie>