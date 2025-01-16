using UnityEngine;
using UnityEngine.Events;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiTowers : MonoBehaviour        // Only one should exist AT ALL TIMES
    {
        [Header("Balls")] [SerializeField] private GameObject bottomBall;
        [SerializeField] private GameObject middleBall;
        [SerializeField] private GameObject topBall;

        [Header("Colliders")] [SerializeField] private GameObject detectorBL;
        // ReSharper disable InconsistentNaming because it's easier to read in this case
        [SerializeField] private GameObject detectorML;
        [SerializeField] private GameObject detectorTL;
        [SerializeField] private GameObject detectorBM;
        [SerializeField] private GameObject detectorMM;
        [SerializeField] private GameObject detectorTM;
        [SerializeField] private GameObject detectorBR;
        [SerializeField] private GameObject detectorMR;
        [SerializeField] private GameObject detectorTR;

        private UnityEvent<GameObject, HanoiCollider> _ballEnterBoxEvent;

        private HanoiBall _bBall;
        private HanoiBall _mBall;
        private HanoiBall _tBall;

        // ReSharper disable NotAccessedField.Local because initializing them is a use due to static properties
        private HanoiCollider _colliderBL;
        private HanoiCollider _colliderML;
        private HanoiCollider _colliderTL;
        private HanoiCollider _colliderBM;
        private HanoiCollider _colliderMM;
        private HanoiCollider _colliderTM;
        private HanoiCollider _colliderBR;
        private HanoiCollider _colliderMR;
        private HanoiCollider _colliderTR;

        private void Awake()
        {
            _ballEnterBoxEvent = new UnityEvent<GameObject, HanoiCollider>(); // Needs to be initialized first as others depend on it
        }

        private void Start() // When game gets loaded
        {
            // Add event listeners
            _ballEnterBoxEvent.AddListener(OnBallEnterBox);

            // Get ball objects
            _bBall = new HanoiBall(bottomBall, 2);
            _mBall = new HanoiBall(middleBall, 1);
            _tBall = new HanoiBall(topBall, 0);
            HanoiBall.AddHanoiBalls(_bBall, _mBall, _tBall);

            // Get collision boxes
            _colliderBL = new HanoiCollider(detectorBL, 0, 0, _ballEnterBoxEvent);
            _colliderML = new HanoiCollider(detectorML, 1, 0, _ballEnterBoxEvent);
            _colliderTL = new HanoiCollider(detectorTL, 2, 0, _ballEnterBoxEvent);
            _colliderBM = new HanoiCollider(detectorBM, 0, 1, _ballEnterBoxEvent);
            _colliderMM = new HanoiCollider(detectorMM, 1, 1, _ballEnterBoxEvent);
            _colliderTM = new HanoiCollider(detectorTM, 2, 1, _ballEnterBoxEvent);
            _colliderBR = new HanoiCollider(detectorBR, 0, 2, _ballEnterBoxEvent);
            _colliderMR = new HanoiCollider(detectorMR, 1, 2, _ballEnterBoxEvent);
            _colliderTR = new HanoiCollider(detectorTR, 2, 2, _ballEnterBoxEvent);

            // Reset ball positions
            _bBall.Object.transform.localPosition = new Vector3(2.5f, -1.5f, 1.25f);
            _mBall.Object.transform.localPosition = new Vector3(2.5f, 0.25f, 1.25f);
            _tBall.Object.transform.localPosition =
                new Vector3(2.5f, 3.5f, 1.25f); // Put higher so it enters its collision last

            // Initial ball positions
            HanoiCollider.ColliderGrid[0, 0].ContainedBall = _bBall;
            HanoiCollider.ColliderGrid[0, 1].ContainedBall = _mBall;
            HanoiCollider.ColliderGrid[0, 2].ContainedBall = _tBall;
        }

        private void OnBallEnterBox(GameObject ballObject, HanoiCollider box)
        {
            // Debug.Log($"{ballObject.name} entered {box.Object.name}");
            HanoiBall ball = HanoiBall.GetHanoiBall(ballObject);
            if (box.ContainedBall == null
                && (box.Height == 0 ||
                    HanoiCollider.ColliderGrid[box.Bar, box.Height - 1].ContainedBall?.Weight > ball.Weight))
            {
                Debug.Log($"Put ball {ballObject.name} at position ({box.Bar}, {box.Height}){(box.Height == 0 ? "" : $" on top of {HanoiCollider.ColliderGrid[box.Bar, box.Height - 1].ContainedBall.Object.name}")}.");
                HanoiCollider.RemoveBall(ball);
                box.ContainedBall = ball;
            }

            // Check win condition
            bool winCondition = true;
            for (int layer = 0; layer < 3; layer++)
                if (HanoiCollider.ColliderGrid[2, layer].ContainedBall?.Weight != 2 - layer)
                    winCondition = false;

            if (winCondition)
                Debug.Log("Game won!");
        }
    }
}