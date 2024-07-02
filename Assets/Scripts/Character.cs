using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class Character : MonoBehaviour
{
    #region Variables & Properties

    #region Local
    [SerializeField] float movementSpeed;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        EventsManager.NodeClicked += MoveTo;
    }

    private void OnDisable()
    {
        EventsManager.NodeClicked -= MoveTo;
    }
    #endregion

    #region Methods
    public void MoveTo(Node[] path)
    {
        Debug.Log("Clicked node named: " + path[path.Length-1].name);
        StartCoroutine(PathWalkCoroutine(path));
    }

    IEnumerator PathWalkCoroutine(Node[] path)
    {
        for (int i = 0; i < path.Length; i++) 
        {
            transform.DOMove(path[i].transform.position, movementSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(movementSpeed);
        }
        yield return null;

        transform.position = path[path.Length - 1].transform.position;
    }
    #endregion
}
