using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;
    
    DialogManager dm;

    public void Start()
    {
        dm = FindObjectOfType<DialogManager>();
    }

    public void TriggerDialog()
    {
        if (!dm.started) {
            foreach (GameObject portraitGO in dialog.portraitGameObjects)
                portraitGO.SetActive(false);
            dm.StartDialog(dialog);
        }
    }

    public void NextDialog(DialogData dd, DialogBoxData[] dbds) 
    {
        if (!dm.started)
        {
            dialog.dialogData = dd;
            dialog.dialogBoxDatas.AddRange(dbds);
            foreach (GameObject portraitGO in dialog.portraitGameObjects)
                portraitGO.SetActive(false);
            dm.StartDialog(dialog);
        }
    }
}
