using OLS_HyperCasual;
using UnityEngine;

public class ChangePlaceModel : BaseModel<ChangePlaceView>
{
    public TeammateModel Teammate { get; private set; }
    public Transform CachedTransform { get; private set; }
    public int IndexPlace;
    
    private BarrierModel Barrier;
    
    public ChangePlaceModel(ChangePlaceView view, BarrierModel barrierModel, int index)
    {
        View = view;
        Barrier = barrierModel;
        IndexPlace = index;
        
        if (barrierModel != null)
        {
            Barrier = barrierModel;
            Barrier.OnDestroyBarrier += SetBrokeTeammate;
        }
        
        CachedTransform = view.transform;
    }

    public void SetTeammate(TeammateModel model)
    {
        PlayerPrefs.SetInt($"Place_{IndexPlace}", (int)model.View.Type);
        Teammate = model;
        model.SetPlace(CachedTransform.position, Barrier);

        if (IsStash() == false)
        {
            SetTypeText($"{(int)Teammate.View.Type}");
        }
    }

    public void ClearPlace()
    {
        PlayerPrefs.SetInt($"Place_{IndexPlace}", 0);
        Teammate = null;
        
        if (IsStash() == false)
        {
            SetTypeText("");
        }
    }

    public void SetTypeText(string text)
    {
        if (View.TextType == null)
        {
            return;
        }
        
        View.TextType.text = text;
    }
    
    public bool IsStash()
    {
        return Barrier == null;
    }
    
    private void SetBrokeTeammate()
    {
        if (Teammate == null)
        {
            return;
        }
        
        Teammate.InAttackPlace = false;
        Teammate.CachedTransform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        Teammate.View.ChangeStateGun(false);
        
        if (Teammate.IsDrag == false)
        {
            Teammate.SetAnimation("isSit");
        }
    }
}