using System;

namespace FultEngine.Module.BackGun;

[Serializable]
public class Config
{
	public bool Debug { get; set; } = false;


	public string E11SchematicName { get; set; } = "E11SR";


	public string FRMG0SchematicName { get; set; } = "FRMG0";


	public string FSP9SchematicName { get; set; } = "FSP9";


	public string CrossvecSchematicName { get; set; } = "Crossvec";


	public string LogicerSchematicName { get; set; } = "MG5";


	public string AKSchematicName { get; set; } = "AK";


	public string Com18SchematicName { get; set; } = "COM18";


	public string RevolverSchematicName { get; set; } = "Revolver";


	public string ShotgunSchematicName { get; set; } = "Shotgun";

}
