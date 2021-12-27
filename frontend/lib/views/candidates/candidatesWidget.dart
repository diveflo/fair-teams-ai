import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/candidates/candidatesCountWidget.dart';
import 'package:no_cry_babies/views/candidates/candidatesListWidget.dart';

import '../appLayoutWidget.dart';

class CandidatesWidget extends StatefulWidget {
  @override
  _CandidatesWidgetState createState() => _CandidatesWidgetState();
}

class _CandidatesWidgetState extends State<CandidatesWidget> {
  ScrollController _scrollController;

  @override
  initState() {
    _scrollController = ScrollController();
    super.initState();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 60),
      child: LayoutBuilder(
          builder: (BuildContext context, BoxConstraints constraints) {
        if (constraints.maxHeight > 400) {
          return Column(
            children: [
              CandidateCountWidget(),
              CandidatesListWidget(scrollController: _scrollController)
            ],
          );
        }
        return Row(
          children: [
            CandidatesListWidget(scrollController: _scrollController),
            CandidateCountWidget(),
          ],
        );
      }),
    );
  }
}
