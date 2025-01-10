using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringAdapter : Adapter
    {

        private Kindy2ColoringConversationData PlayData;

        public override T GetData<T>(int turn)
        {
            T data;

            Type listType = typeof(T);
            if (listType == typeof(Kindy2ColoringInitStateData))
            {
                Kindy2ColoringInitStateData initStateData = new Kindy2ColoringInitStateData();
                initStateData.DataPlay = PlayData;
                data = ConvertToType<T>(initStateData);
            }
            else if (listType == typeof(Kindy2ColoringOutroStateData))
            {
                Kindy2ColoringOutroStateData outroStateData = new Kindy2ColoringOutroStateData();
                outroStateData.DataPlay = PlayData;
                data = ConvertToType<T>(outroStateData);
            }
            else
            {
                data = ConvertToType<T>(null);
            }

            return data;
        }

        public override int GetMaxTurn()
        {
            return 1;
        }

        public override void SetData<T>(T data)
        {
            PlayData = ConvertToType<Kindy2ColoringConversationData>(data);
        }
    }
}