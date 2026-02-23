using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OrganizedResearchTech
{
    public class Utils
    {
        public static void DoCheckButton(Rect rect, Texture2D buttonTex, string tooltip, ref bool enabled)
        {
            if (Widgets.ButtonImage(rect, buttonTex))
            {
                enabled = !enabled;
                if (enabled)
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
            }
            TooltipHandler.TipRegion(rect, tooltip);
            Rect checkRect = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height / 2f);
            GUI.DrawTexture(checkRect, enabled ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
        }

        public static void DoDraggable(int group, Rect draggableRect, Rect? controlRect = null, Rect? tipRect = null)
        {
            if (controlRect.HasValue)
            {
                using (new TextBlock(Mouse.IsOver(controlRect.Value) && !ReorderableWidget.Dragging ? GenUI.MouseoverColor : Color.white)) GUI.DrawTexture(controlRect.Value, TexButton.DragHash);
            }
            if (tipRect.HasValue)
            {
                TooltipHandler.TipRegion(tipRect.Value, "Defaults_DragToReorder".Translate());
            }
            if ((Event.current.type != EventType.MouseDown || Mouse.IsOver(controlRect ?? draggableRect)) && ReorderableWidget.Reorderable(group, draggableRect))
            {
                Widgets.DrawRectFast(draggableRect, Color.black.WithAlpha(0.5f));
            }
        }

        public static void IntEntry(Rect rect, ref int value, ref string editBuffer, int multiplier = 1, int minimum = 0, int maximum = int.MaxValue)
        {
            Widgets.IntEntry(rect, ref value, ref editBuffer, multiplier);
            value = Mathf.Clamp(value, minimum, maximum);
            editBuffer = value.ToString();
        }
    }
}
