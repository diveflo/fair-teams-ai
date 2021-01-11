import 'dart:math';

import 'package:confetti/confetti.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/thunks/scramble.dart';
import 'package:frontend/views/myButtonWidget.dart';

class ScrambleWidget extends StatefulWidget {
  const ScrambleWidget({
    Key key,
  }) : super(key: key);

  @override
  _ScrambleWidgetState createState() => _ScrambleWidgetState();
}

class _ScrambleWidgetState extends State<ScrambleWidget> {
  ConfettiController _confettiController;

  @override
  initState() {
    _confettiController = ConfettiController(duration: Duration(seconds: 3));
    super.initState();
  }

  @override
  void dispose() {
    _confettiController.dispose();
    super.dispose();
  }

  void _scrambleApi({bool hltv = false}) {
    StoreProvider.of<AppState>(context)
        .dispatch(scrambleTeams(hltv, _confettiController));
  }

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, bool>(
      converter: (store) => store.state.gameState.isLoading,
      builder: (context, isLoading) {
        return Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 10),
              child: MyButtonWidget(
                buttonText: "Scramble",
                onPressed: _scrambleApi,
                color: Colors.lime,
              ),
            ),
            isLoading ? CircularProgressIndicator() : Container(),
            ConfettiWidget(
              confettiController: _confettiController,
              blastDirection: pi,
              colors: [Colors.black, Colors.orange],
              maxBlastForce: 40,
              numberOfParticles: 30,
              particleDrag: 0.01,
              gravity: 0.07,
            ),
          ],
        );
      },
    );
  }
}
