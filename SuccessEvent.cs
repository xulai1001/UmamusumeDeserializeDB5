﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmamusumeDeserializeDB5
{
    public static class SuccessEvent
    {
        public static void Generate(List<Story> stories)
        {
            var successEvent = new List<SuccessStory>();
            //吃饭
            successEvent.AddRange(stories.Where(x => x.Choices.Count == 2 && x.Choices[1].SuccessEffect == "体力+30、スキルPt+10").Select(x => new SuccessStory
            {
                Name = x.Name,
                Choices = new List<SuccessChoice>
                 {
                     new SuccessChoice
                     {
                           ChoiceIndex=2,
                           SelectIndex=1,
                           Effects=new SuccessChoiceEffectDictionary
                           {
                               { 0, "体力+30、スキルPt+10" }
                           }
                     }
                 }
            }));
            //无选项事件且随机给不同技能hint
            successEvent.AddRange(stories.Where(x => x.Choices.Count == 1 && x.Choices[0].SuccessEffect.Contains("ヒントLv") && x.Choices[0].FailedEffect.Contains("ヒントLv")).Select(x => new SuccessStory
            {
                Name = x.Name,
                Choices = new List<SuccessChoice>
                 {
                     new SuccessChoice
                     {
                           ChoiceIndex=1,
                           SelectIndex=1,
                           Effects=new SuccessChoiceEffectDictionary
                           {
                               { 0, x.Choices[0].SuccessEffect }
                           }
                     }
                 }
            }));

            File.WriteAllText($"output/successevent.json", JsonConvert.SerializeObject(successEvent, Formatting.Indented));
        }
    }
    public class SuccessStory
    {
        public string Name { get; set; } = string.Empty;
        public List<SuccessChoice> Choices { get; set; } = new();
    }
    public class SuccessChoice
    {
        /// <summary>
        /// 服务器下发的ChoiceIndex
        /// </summary>
        public int ChoiceIndex { get; set; }
        /// <summary>
        /// 服务器下发的SelectIndex，即第几个选项
        /// </summary>
        public int SelectIndex { get; set; }
        /// <summary>
        /// 事件效果，Key为限定的剧本ID（部分事件需要，如赛后事件），不限定剧本ID的Key统一为0
        /// </summary>
        public SuccessChoiceEffectDictionary Effects { get; set; } = new();//ScenarioId-Effect
    }
    public class SuccessChoiceEffectDictionary : Dictionary<int, string>
    {
        public new bool ContainsKey(int key) => base.ContainsKey(key) || base.ContainsKey(0);
        public new string this[int key]
        {
            get => base.ContainsKey(key) ? base[key] : base[0]; //如果有对应剧本的效果则返回，否则返回通用
            set => base[key] = value;
        }
    }
}