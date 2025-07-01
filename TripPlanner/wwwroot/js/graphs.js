let cyInstance;

window.renderGraph = function (graphData, type) {
    console.log("📊 Graph data:", graphData);

    const nodes = graphData.nodes.map(n => ({
        data: {
            id: n.id,
            label: n.id,
            highlighted: n.highlighted
        }
    }));

    const edges = graphData.links.map(l => ({
        data: {
            source: l.source,
            target: l.target,
            price: l.price,
            time: l.time,
            emissions: l.emissions,
            highlighted: l.highlighted,
            label: l.label
        }
    }));

    cyInstance = cytoscape({
        container: document.getElementById(`graph-${type}`),
        elements: nodes.concat(edges),
        layout: { name: 'circle' },
        style: [
            {
                selector: 'node',
                style: {
                    'label': 'data(label)',
                    'background-color': '#007bff',
                    'color': '#fff',
                    'text-valign': 'center',
                    'text-halign': 'center'
                }
            },
            {
                selector: 'node[highlighted = "true"]',
                style: {
                    'background-color': 'green'
                }
            },
            {
                selector: 'edge',
                style: {
                    'width': 2,
                    'line-color': '#ccc',
                    'curve-style': 'bezier',
                    'target-arrow-shape': 'triangle',
                    'target-arrow-color': '#ccc'
                }
            },
            {
                selector: 'edge[label]',
                style: {
                    'label': 'data(label)',
                    'font-size': '11px',
                    'color': '#fff',
                    'text-rotation': 'autorotate',
                    'text-margin-y': '-6px',

                    // Opções visuais mais leves e suaves
                    'text-background-color': '#fff',
                    'text-background-opacity': 0.8,
                    'text-background-shape': 'roundrectangle',
                    'text-background-padding': '2px',
                    'text-border-opacity': 1,
                    'text-background-opacity': 0
                }
            },
            {
                selector: 'edge[highlighted = "true"]',
                style: {
                    'width': 4,
                    'line-color': '#27ae60',
                    'target-arrow-color': '#27ae60'
                }
            }
        ]
    });
};

window.setupGraphInterop = function (dotNetHelper) {
    if (!cyInstance) return;

    cyInstance.on('tap', 'node', function (evt) {
        const nodeId = evt.target.id();
        dotNetHelper.invokeMethodAsync('ShowCityModal', nodeId);
    });
};