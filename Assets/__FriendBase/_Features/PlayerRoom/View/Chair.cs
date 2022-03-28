using System;
using UnityEngine;

namespace PlayerRoom.View
{
    public class Chair : RoomItem
    {
        public bool facingRight;

        [SerializeField] float sitY = 0.5f;
        [SerializeField] float sitX = 0f;
        [SerializeField] float chairSittingLeaway = 0.1f;
        public Vector2 SitPoint => transform.position + (Vector3)(transform.localScale * new Vector2(sitX, sitY));

        private bool occupied = false;
        public float GetChairSittingLeaway()
        {
            return chairSittingLeaway;
        }

        public void SetChairOccupied(bool occupied)
        {
            this.occupied = occupied;
        }

        public bool IsOccupied()
        {
            return this.occupied;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.4f);
            Gizmos.DrawCube(SitPoint, new Vector2(0.25f, 0.05f));
        }
    }
}