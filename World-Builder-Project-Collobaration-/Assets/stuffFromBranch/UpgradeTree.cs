using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeTree : MonoBehaviour
{
    public GameObject upgradePanel;
     public GameObject NaturePanel;
    public GameObject HerbsPanel;
    public GameObject CarnPanel;
    public TextMeshProUGUI herbTx;
    public TextMeshProUGUI carnTx;
    public TextMeshProUGUI rabbitTx;
    public TextMeshProUGUI tallsTx;
    public TextMeshProUGUI hugeTx;
    public TextMeshProUGUI raptorTx;
    public TextMeshProUGUI birdsTx;

    public GameObject[] herbivores;
     public GameObject[] carnivores;
     public GameObject[] rabbits;
     public GameObject[] tallHerbivore;
      public GameObject[] hugeHerbivore;
       public GameObject[] raptor;
     public GameObject[] birds; 

     // writing all again for plants

      public TextMeshProUGUI grasstx;
      public TextMeshProUGUI talltreetx;
      public TextMeshProUGUI redBushtx;
      public TextMeshProUGUI bluBushtx;

       public TextMeshProUGUI whiteBushtx;

      public TextMeshProUGUI cactustx;
       public TextMeshProUGUI cactusTreetx;

       public TextMeshProUGUI palmTreetx;

        public TextMeshProUGUI pineTreetx;

         public TextMeshProUGUI largeTreetx; 

      //gameObjects lists

      public GameObject[] grass;
      public GameObject[] whiteBush;

      public GameObject[] bluBush;

       public GameObject[] redBush;

       public GameObject[] palmTree;

      public GameObject[] pineTree;

       public GameObject[] tallTree;

        public GameObject[] cactus;

         public GameObject[] cactusTree;

        public GameObject[] largeTree;  



     public GameObject lockedButton1;
     public GameObject lockedButton2;  

    public bool isLv1;
    public bool isLv2;
    

    public void Update()
    {
         if(Input.GetMouseButtonDown(1))
         {
           
            upgradePanel.SetActive(false);
         }
         if(Healthbar.darwinPoints >= 50)
         {
            isLv1 = true;
            lockedButton1.SetActive(false);
         }
        
    }
    public void UpdateUI()
    {
         herbivores = GameObject.FindGameObjectsWithTag("Herbivore");
         herbTx.text = herbivores.Length.ToString("0");
        carnivores = GameObject.FindGameObjectsWithTag("Carnivore");
        carnTx.text = carnivores.Length.ToString("0");
        rabbits = GameObject.FindGameObjectsWithTag("small herbivore");
        rabbitTx.text = rabbits.Length.ToString("0");
        tallHerbivore = GameObject.FindGameObjectsWithTag("tall herbivore");
        tallsTx.text = tallHerbivore.Length.ToString("0");
        hugeHerbivore = GameObject.FindGameObjectsWithTag("hugeHerbivore");
        hugeTx.text = hugeHerbivore.Length.ToString("0"); 
        raptor = GameObject.FindGameObjectsWithTag("raptor");
        raptorTx.text = raptor.Length.ToString("0");
        birds = GameObject.FindGameObjectsWithTag("bird");
        birdsTx.text = birds.Length.ToString("0");

        // now repeating for the Plants

        grass = GameObject.FindGameObjectsWithTag("Plant");
        grasstx.text = grass.Length.ToString("0");
        whiteBush = GameObject.FindGameObjectsWithTag("blackBush");
        whiteBushtx.text = whiteBush.Length.ToString("0");
        redBush = GameObject.FindGameObjectsWithTag("redBush");
        redBushtx.text = redBush.Length.ToString("0");
        bluBush = GameObject.FindGameObjectsWithTag("bluBush");
        bluBushtx.text = bluBush.Length.ToString("0");
        palmTree = GameObject.FindGameObjectsWithTag("PalmTree");
        palmTreetx.text = palmTree.Length.ToString("0");
        pineTree = GameObject.FindGameObjectsWithTag("PineTree");
        pineTreetx.text = pineTree.Length.ToString("0");
        largeTree = GameObject.FindGameObjectsWithTag("LargeTree");
        largeTreetx.text = largeTree.Length.ToString("0");
        tallTree = GameObject.FindGameObjectsWithTag("tree");
        talltreetx.text = tallTree.Length.ToString("0");
        cactus = GameObject.FindGameObjectsWithTag("Cactus");
        cactustx.text = cactus.Length.ToString("0");
        cactusTree = GameObject.FindGameObjectsWithTag("CactusTree");
        cactusTreetx.text = cactusTree.Length.ToString("0");
    }

    public void OpenPanel()
    {
       upgradePanel.SetActive(true);
      
       UpdateUI();
    }
    public void ClosePanel()
    {
       upgradePanel.SetActive(false);
    }
    public void OpenHerbPanel()
    {
        HerbsPanel.SetActive(true);
        upgradePanel.SetActive(false);
        
    }
     public void OpenCarnPanel()
     {
        CarnPanel.SetActive(true);
        upgradePanel.SetActive(false);
     }

     public void CloseCarnPanel()
     {
       CarnPanel.SetActive(false);
        upgradePanel.SetActive(true); 
     }
    public void CloseHerbPanel()
    {
        HerbsPanel.SetActive(false);
        upgradePanel.SetActive(true);
    }
    public void OpenNaturePanel()
    {
      NaturePanel.SetActive(true);
      upgradePanel.SetActive(false);
    }
    public void CloseNaturePanel()
    {
      NaturePanel.SetActive(false);
      upgradePanel.SetActive(true);
    }
}
