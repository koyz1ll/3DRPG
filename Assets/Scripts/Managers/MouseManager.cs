using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MouseManager : Singleton<MouseManager>
{
   private RaycastHit hitInfo;
   public Action<Vector3> OnMouseClicked;
   public Action<GameObject> OnEnemyClicked;
   public Texture2D point, doorway, attack, target, arrow;

   void Update()
   {
      SetCursorTexture();
      MouseControl();
   }

   void SetCursorTexture()
   {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
      if (Physics.Raycast(ray, out hitInfo))
      {
         //切换鼠标贴图
         var hitLayer = hitInfo.collider.gameObject.layer;
         if (hitLayer == LayerUtils.Ground)
         {
            Cursor.SetCursor(target, new Vector2(16,16), CursorMode.Auto);
         }
         else if (hitLayer == LayerUtils.Enemy || hitLayer == LayerUtils.Attackable)
         {
            Cursor.SetCursor(attack, new Vector2(16,16), CursorMode.Auto);
         }
         
      }
   }

   void MouseControl()
   {
      if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
      {
         if (hitInfo.collider.gameObject.layer.Equals(LayerUtils.Ground))
         {
            OnMouseClicked?.Invoke(hitInfo.point);
         }

         if (hitInfo.collider.gameObject.layer.Equals(LayerUtils.Enemy) || hitInfo.collider.gameObject.layer.Equals(LayerUtils.Attackable))
         {
            OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
         }
      }
   }
}
