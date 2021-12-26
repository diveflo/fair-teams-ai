import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/state/appState.dart';

class BotsWidget extends StatelessWidget {
  _onValueChanged(BuildContext context) {
    StoreProvider.of<AppState>(context).dispatch(ToggleincludeBotAction());
  }

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, bool>(
        converter: (store) => store.state.gameConfigState.includeBot,
        builder: (context, includeBot) {
          return Padding(
            padding: const EdgeInsets.symmetric(vertical: 15),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  "Include Bot?",
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                Switch(
                  value: includeBot,
                  onChanged: (bool isOn) {
                    _onValueChanged(context);
                  },
                ),
              ],
            ),
          );
        });
  }
}
