using UnityEngine;


namespace KoreKrush
{
    [CreateAssetMenu(fileName = "Generate Tile", menuName = "Kore Krush/Motors Abilitys/Create GenerateTile Motor Ability")]
    public class GenerateTileAbility : MotorAbility
    {
        public GameObject TileGenerated;


        public override void DoAction(params object[] content)
        {
            if (TileGenerated != null)
            {
                Events.Logic.MotorTileSpawn(TileGenerated);
            }
        }

        public void OnEnable()
        {
            if (TileGenerated)
                abilityImg = TileGenerated.GetComponent<SpriteRenderer>().sprite;
        }
    }

}