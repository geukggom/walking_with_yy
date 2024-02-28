using Generated;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class FlowBlockBase : MonoBehaviour
{
    [SerializeField, CanBeNull] private Image _titleImage;

    public virtual void OnShow(FlowManagementData data)
    {
        if (data.PlayingBgm != string.Empty)
        {
            SoundManager.Instance.PlayBgm(data.PlayingBgm);
        }

        if (_titleImage != null)
        {
            if (data.TitleResourceKey != string.Empty)
            {
                var image = Resources.Load<Sprite>(data.TitleResourceKey);
                var hasImage = image != null;
                _titleImage.gameObject.SetActive(hasImage);
                
                if (hasImage)
                {
                    _titleImage.sprite = image;
                }
            }
            else
            {
                _titleImage.gameObject.SetActive(false);
            }
        }
    }

    public virtual void OnHide()
    {
    }
}