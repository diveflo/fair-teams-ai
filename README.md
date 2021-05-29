# fair-teams-ai

![backend ci/cd](https://github.com/diveflo/fair-teams-ai/workflows/backend%20ci/cd/badge.svg) ![frontend ci/cd](https://github.com/diveflo/fair-teams-ai/workflows/frontend%20ci/cd/badge.svg)

Tired of listening to your friends whine about how unfair your CS:GO teams are? Enraged by the arrays, I'm sorry..."matrices", that start at `1` of your buddy's MATLAB script?
*Fair Teams AI* automagically creates balanced team assignments for your next match.

<https://diveflo.github.io/fair-teams-ai/#/>

## Development

The frontend is developed using flutter. You can get more details in the [frontend-specific README](https://github.com/diveflo/fair-teams-ai/blob/main/frontend/README.md).
The backend is using .NET 5.0 and is providing a REST API to communicate with the frontend. More details in the [backend-specific README](https://github.com/diveflo/fair-teams-ai/blob/main/backend/README.md).

## Deployment

The frontend can be deployed on any http host, e.g., GitHub Pages as is done for our version. The backend only requires a bit of processing power. Our current deployment runs on a 1 vCPU, 1 GB RAM shared instance host on [linode](https://www.linode.com/products/shared/).
