// Interaction.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

using TriggerAction = System.Windows.Interactivity.TriggerAction;
using TriggerCollection = System.Windows.Interactivity.TriggerCollection;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A class providing various attached properties for creating interactivty.
    /// </summary>
    public static class Interaction {

        /// <summary>
        /// Represents the Action attached property.
        /// </summary>
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.RegisterAttached("Action", typeof(TriggerAction), typeof(Interaction), null);

        /// <summary>
        /// Represents the Behaviors attached property.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), null);

        /// <summary>
        /// Represents the Command attached property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(string), typeof(Interaction), null);

        /// <summary>
        /// Represents the Triggers attached property.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(TriggerCollection), typeof(Interaction), null);

        /// <summary>
        /// Gets the action associated with the specified Button.
        /// </summary>
        /// <param name="button">The Button to lookup.</param>
        /// <returns>The action associated with the Button; null if there is none.</returns>
        public static TriggerAction GetAction(ButtonBase button) {
            TriggerCollection triggers = (TriggerCollection)button.GetValue(TriggersProperty);
            if (triggers != null) {
                foreach (Trigger trigger in triggers) {
                    ClickTrigger clickTrigger = trigger as ClickTrigger;
                    if (clickTrigger != null) {
                        return clickTrigger.Action;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the collection of behaviors for the specified DependencyObject.
        /// </summary>
        /// <param name="o">The DependencyObject to lookup.</param>
        /// <returns>The collection of associated behaviors.</returns>
        public static BehaviorCollection GetBehaviors(DependencyObject o) {
            BehaviorCollection behaviors = (BehaviorCollection)o.GetValue(BehaviorsProperty);
            if (behaviors == null) {
                behaviors = new BehaviorCollection(o);
                SetBehaviors(o, behaviors);
            }

            return behaviors;
        }

        /// <summary>
        /// Gets the command name associated with the specified Button.
        /// </summary>
        /// <param name="button">The Button to lookup.</param>
        /// <returns>The command name associated with the Button; null if there is none.</returns>
        public static string GetCommand(ButtonBase button) {
            BehaviorCollection behaviors = (BehaviorCollection)button.GetValue(BehaviorsProperty);
            if (behaviors != null) {
                foreach (Behavior behavior in behaviors) {
                    Command commandBehavior = behavior as Command;
                    if (commandBehavior != null) {
                        return commandBehavior.CommandName;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the collection of triggers for the specified DependencyObject.
        /// </summary>
        /// <param name="o">The DependencyObject to lookup.</param>
        /// <returns>The collection of associated triggers.</returns>
        public static TriggerCollection GetTriggers(DependencyObject o) {
            TriggerCollection triggers = (TriggerCollection)o.GetValue(TriggersProperty);
            if (triggers == null) {
                triggers = new TriggerCollection(o);
                SetTriggers(o, triggers);
            }

            return triggers;
        }

        /// <summary>
        /// Sets the action associated with the specified Button.
        /// </summary>
        /// <param name="button">The Button to associate the action with.</param>
        /// <param name="action">The action to associate with the button.</param>
        public static void SetAction(ButtonBase button, TriggerAction action) {
            if (action == null) {
                return;
            }

            TriggerCollection triggers = GetTriggers(button);

            foreach (Trigger trigger in triggers) {
                ClickTrigger clickTrigger = trigger as ClickTrigger;
                clickTrigger.Action = action;
            }

            ClickTrigger newTrigger = new ClickTrigger();
            newTrigger.Action = action;

            triggers.Add(newTrigger);
        }

        /// <summary>
        /// Sets the collection of behaviors for the specified DependencyObject.
        /// </summary>
        /// <param name="o">The DependencyObject to set.</param>
        /// <param name="behaviors">The collection of behaviors to associate.</param>
        public static void SetBehaviors(DependencyObject o, BehaviorCollection behaviors) {
            o.SetValue(BehaviorsProperty, behaviors);
        }

        /// <summary>
        /// Sets the name of the command associated with the specified Button.
        /// </summary>
        /// <param name="button">The Button to associate the command with.</param>
        /// <param name="commandName">The name of the command to associated.</param>
        public static void SetCommand(ButtonBase button, string commandName) {
            BehaviorCollection behaviors = GetBehaviors(button);
            foreach (Behavior behavior in behaviors) {
                Command commandBehavior = behavior as Command;
                if (commandBehavior != null) {
                    if (String.IsNullOrEmpty(commandName)) {
                        behaviors.Remove(commandBehavior);
                    }
                    else {
                        commandBehavior.CommandName = commandName;
                    }
                    return;
                }
            }

            if (String.IsNullOrEmpty(commandName) == false) {
                Command commandBehavior = new Command();
                commandBehavior.CommandName = commandName;

                behaviors.Add(commandBehavior);
            }
        }

        /// <summary>
        /// Sets the collection of triggers for the specified DependencyObject.
        /// </summary>
        /// <param name="o">The DependencyObject to set.</param>
        /// <param name="triggers">The collection of triggers to associate.</param>
        public static void SetTriggers(DependencyObject o, TriggerCollection triggers) {
            o.SetValue(TriggersProperty, triggers);
        }
    }
}
