using UnityEngine;

public class BuildingSound : MonoBehaviour {

    [SerializeField] private MyGridBuildingSystem myGridBuildingSystem = null;
    [SerializeField] private Transform pfBuildingSound = null;

    private void Start() {
        if (myGridBuildingSystem != null) {
            myGridBuildingSystem.OnObjectPlaced += GridBuildingSystem3D_OnObjectPlaced;
        }
    }

    private void GridBuildingSystem3D_OnObjectPlaced(object sender, System.EventArgs e) {
        Transform buildingSoundTransform = Instantiate(pfBuildingSound, Vector3.zero, Quaternion.identity);
        Destroy(buildingSoundTransform.gameObject, 2f);
    }

    private void GridBuildingSystem2D_OnObjectPlaced(object sender, System.EventArgs e) {
        Transform buildingSoundTransform = Instantiate(pfBuildingSound, Vector3.zero, Quaternion.identity);
        Destroy(buildingSoundTransform.gameObject, 2f);
    }

}
